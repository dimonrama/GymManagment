

using GymManagment.Application.Interfaces;
using GymManagment.Domain.Common;
using GymManagment.Domain.DTO;
using GymManagment.Domain.Models;
using GymManagment.Infrastructure.Data;
using Microsoft.EntityFrameworkCore;
using Microsoft.IdentityModel.Tokens;
using System.IdentityModel.Tokens.Jwt;
using System.Net;
using System.Security.Claims;
using System.Security.Cryptography;
using System.Text;
using RabbitMQ.Client;
using System.Text.Json;


namespace GymManagment.Application.Services
{
    public class AuthService : IAuthService
    {
        private readonly GymDbContext _context;
        private readonly IConfiguration _config;
        private readonly ILogger<AuthService> _logger;
        private readonly IConnection _rabbitConnection;
        public AuthService(GymDbContext context, IConfiguration config, ILogger<AuthService> logger, IConnection rabbitConnection)
        {
            _context = context;
            _config = config;
            _logger = logger;
            _rabbitConnection = rabbitConnection;
        }

        public async Task<Result> RegisterAsync(RegisterDto dto)
        {
            bool existName = await _context.Users.AnyAsync(u => u.Username == dto.Username);
            if (existName)
            {
                _logger.LogWarning("Имя занято: {Username}", dto.Username);
                return Result.Fail("Такое имя уже занято", Result.ErrorTypes.Conflict);
            }

            if (!Enum.TryParse<UserRole>(dto.Role, out var role))
            {
                _logger.LogWarning("Несовпадение роли: {Role}", dto.Role);
                return Result.Fail("Роль должна быть равна Trainer,Member или Admin", Result.ErrorTypes.ValidationError);
            }

            // Проверки профиля клиента делаем ДО записи чего-либо в БД
            if (role == UserRole.Member)
            {
                if (await _context.Members.AnyAsync(m => m.Email == dto.Email))
                {
                    _logger.LogWarning("Почта занята: {Email}", dto.Email);
                    return Result.Fail("Такая почта уже зарегистрирована", Result.ErrorTypes.Conflict);
                }

                if (dto.TrainerId.HasValue && !await _context.Trainers.AnyAsync(t => t.Id == dto.TrainerId.Value))
                {
                    _logger.LogWarning("Тренер не найден: {TrainerId}", dto.TrainerId);
                    return Result.Fail($"Тренер с Id {dto.TrainerId} не найден", Result.ErrorTypes.ValidationError);
                }
            }

            string passwordHash = BCrypt.Net.BCrypt.HashPassword(dto.Password);

            var user = new User
            {
                Username = dto.Username,
                PasswordHash = passwordHash,
                Role = role
            };
            await _context.Users.AddAsync(user);

            // Профиль создаём через навигационное свойство User: EF сам проставит UserId при сохранении
            if (role == UserRole.Member)
            {
                await _context.Members.AddAsync(new Member
                {
                    FullName = dto.FullName!,
                    Age = dto.Age!.Value,
                    Email = dto.Email!,
                    TrainerId = dto.TrainerId,
                    User = user
                });
            }
            else if (role == UserRole.Trainer)
            {
                await _context.Trainers.AddAsync(new Trainer
                {
                    FullName = dto.FullName!,
                    Age = dto.Age!.Value,
                    ExperienceYears = dto.ExperienceYears!.Value,
                    User = user
                });
            }

            // Один SaveChanges = одна транзакция: сохранится и User, и профиль, либо ничего
            var success = await _context.SaveChangesAsync() > 0;
            if (!success)
            {
                _logger.LogError("НЕ удалось сохранить в БД: {Username}", dto.Username);
                return Result.Fail("Не удалось создать пользователя", Result.ErrorTypes.ServerError);
            }

            _logger.LogInformation("Успешная регистрация: {Username}, роль {Role}", dto.Username, role);
            await PublishWelcomeMessageAsync(user.Username);
            return Result.Ok();
        }

        public async Task<TokenResponseDto?> LoginAsync(LoginDto dto)
        {
            var client = await _context.Users.FirstOrDefaultAsync(u => u.Username == dto.Username);
            if (client == null) {
                _logger.LogWarning("Клиент не найден: {Username}", dto.Username);
                return null; }
            bool pass = BCrypt.Net.BCrypt.Verify(dto.Password, client.PasswordHash);
            if (!pass) { _logger.LogWarning("Неверный пароль: {Username}", dto.Username);return null; }
            _logger.LogInformation("Пользователь авторизован: {Username}", dto.Username);
            RefreshToken refreshToken = new RefreshToken()
            {
                UserId = client.Id,
                Token = GenerateRefreshToken(),
                ExpiresAt = DateTime.UtcNow.AddDays(30),
                IsActive = true
            };
            await _context.RefreshTokens.AddAsync(refreshToken);
            await _context.SaveChangesAsync();

            return new TokenResponseDto {
                AccessToken = GenerateToken(client),
                RefreshToken = refreshToken.Token
              
              
            };

            
        }



        public async Task<Result> LogoutAsync(RefreshRequestDto dto)
        {
            var refreshToken = await _context.RefreshTokens.FirstOrDefaultAsync(r=>r.Token == dto.RefreshToken);
            if (refreshToken == null)
            {
                _logger.LogWarning("Попытка логаута с несуществующим токеном {RefreshToken}", dto.RefreshToken);
               
                return Result.Fail("Попытка логаута с несуществующим токеном", Result.ErrorTypes.NotFound);
            }
            refreshToken.IsActive = false;
            await _context.SaveChangesAsync();
            _logger.LogInformation("Токен отозван (logout): UserId {UserId}", refreshToken.UserId);
            return Result.Ok();
        }


        public async Task<TokenResponseDto?> RefreshTokenAsync(RefreshRequestDto dto)
        {
            var refreshToken = await _context.RefreshTokens.Include(r=>r.User).FirstOrDefaultAsync(r=>r.Token == dto.RefreshToken);
            if (refreshToken == null) {
                _logger.LogWarning("Токен не найден: {Token}", dto.RefreshToken);
                return null;
            }
            if (refreshToken.IsActive == false) { 
                _logger.LogWarning("{RefreshToken} - Токен неактивен, попытка захода с этим токеном",dto.RefreshToken);
                var allActiveRefreshTokens = await _context.RefreshTokens.Where(r => r.IsActive == true && r.UserId == refreshToken.UserId).ToListAsync();
                foreach (var r in allActiveRefreshTokens)
                {
                    r.IsActive = false;
                }
                _logger.LogInformation("Все токены {UserId} помечены как неактивные", refreshToken.UserId);
                await _context.SaveChangesAsync();
                return null; }

            if (refreshToken.ExpiresAt<DateTime.UtcNow)
            {
                _logger.LogWarning("{RefreshToken} - Токен устарел, попытка захода с этим токеном",dto.RefreshToken);
                return null;
            }

            var user = refreshToken.User;
            if (user == null) { _logger.LogWarning("Пользователь по токену не найден - {UserId}", refreshToken.UserId);
                return null;
            }
            refreshToken.IsActive = false;
            var newRefreshToken = new RefreshToken()
            {

                UserId = user.Id,
                Token = GenerateRefreshToken(),
                ExpiresAt = DateTime.UtcNow.AddDays(30),
                IsActive = true
            };
            await _context.RefreshTokens.AddAsync(newRefreshToken);
            await _context.SaveChangesAsync(); 
            return new TokenResponseDto
            {
                AccessToken = GenerateToken(user),
                RefreshToken = newRefreshToken.Token
            };
        }
        private string GenerateToken(User user)
        {
            var claims = new[]
                 {
                 new Claim(ClaimTypes.NameIdentifier, user.Id.ToString()),
                 new Claim(ClaimTypes.Name, user.Username),
                 new Claim(ClaimTypes.Role, user.Role.ToString())
                 };
            var secretKey = _config["JwtSettings:SecretKey"];
            var issuer = _config["JwtSettings:Issuer"];
            var audience = _config["JwtSettings:Audience"];
            var expiresInMinutes = int.Parse(_config["JwtSettings:ExpiresInMinutes"]);
            var key = new SymmetricSecurityKey(Encoding.UTF8.GetBytes(secretKey));
            var credentials = new SigningCredentials(key, SecurityAlgorithms.HmacSha256);
            var token = new JwtSecurityToken(
         issuer: issuer,
         audience: audience,
         claims: claims,
         expires: DateTime.UtcNow.AddMinutes(expiresInMinutes),
         signingCredentials: credentials
            );
            return new JwtSecurityTokenHandler().WriteToken(token);
        }
        private string GenerateRefreshToken()
        {
            var randomBytes = new byte[64];
            using var rng = RandomNumberGenerator.Create();
            rng.GetBytes(randomBytes);
            return Convert.ToBase64String(randomBytes);
        }
        private async Task PublishWelcomeMessageAsync(string username)
        {
         
            using var channel = await _rabbitConnection.CreateChannelAsync();

            
            await channel.QueueDeclareAsync(
                queue: "welcome-notifications",
                durable: true,      
                exclusive: false,    
                autoDelete: false,   
                arguments: null);

         
            var message = new { Username = username, RegisteredAtUtc = DateTime.UtcNow };
            string json = JsonSerializer.Serialize(message);
            byte[] body = Encoding.UTF8.GetBytes(json);

           
            await channel.BasicPublishAsync(
                exchange: "",
                routingKey: "welcome-notifications",
                body: body);

            _logger.LogInformation("Сообщение о регистрации отправлено в очередь: {Username}", username);
        }

       public async Task<Result> LinkTelegramAsync(string username, string password, long chatId)
        {
            var client = await _context.Users.FirstOrDefaultAsync(u => u.Username == username);
            if (client == null)
            {
                _logger.LogWarning("Клиент не найден: {Username}", username);
                return Result.Fail($"Клиент не найден {username}", Result.ErrorTypes.NotFound) ;
            }
            bool pass = BCrypt.Net.BCrypt.Verify(password, client.PasswordHash);
            if (!pass) { _logger.LogWarning("Неверный пароль: {Username}", username); return Result.Fail($"Попытка входа с неправильным паролем {username}", Result.ErrorTypes.ValidationError); }
            _logger.LogInformation("Пользователь авторизован: {Username}", username);
            var chatClient = await _context.Users.FirstOrDefaultAsync(c => c.TelegramChatId == chatId);
            if (chatClient != null && chatClient.Id != client.Id)
            {
                chatClient.TelegramChatId = null;
            }
            client.TelegramChatId = chatId;
            await _context.SaveChangesAsync();

           return Result.Ok();
        }
        public async Task<User?> GetUserByTelegramChatIdAsync(long chatId)
        {
            return await _context.Users.AsNoTracking().FirstOrDefaultAsync(u=>u.TelegramChatId == chatId);
        }
    }
}
