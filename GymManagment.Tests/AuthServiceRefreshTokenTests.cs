using GymManagment.Application.Interfaces;
using GymManagment.Application.Services;
using GymManagment.Domain.Common;
using GymManagment.Domain.DTO;
using GymManagment.Domain.Models;
using GymManagment.Infrastructure.Data;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.Logging;
using Moq;

namespace GymManagment.Tests
{
    public class AuthServiceRefreshTokenTests
    {
        private readonly GymDbContext _context;
        private readonly AuthService _authService;

        // Это конструктор КЛАССА теста — xUnit вызывает его сам,
        // заново, перед КАЖДЫМ тестовым методом ([Fact]) в этом классе.
        public AuthServiceRefreshTokenTests()
        {
            _context = GetInMemoryContext();

            var config = GetFakeConfiguration();
            var logger = new Mock<ILogger<AuthService>>().Object;

            _authService = new AuthService(_context, config, logger);
        }

        private GymDbContext GetInMemoryContext()
        {
            var options = new DbContextOptionsBuilder<GymDbContext>()
                .UseInMemoryDatabase(Guid.NewGuid().ToString())
                .Options;
            return new GymDbContext(options);
        }

        private IConfiguration GetFakeConfiguration()
        {
            var inMemorySettings = new Dictionary<string, string>
            {
                { "JwtSettings:SecretKey", "ThisIsAFakeSecretKeyForTestingPurposesOnly123!" },
                { "JwtSettings:Issuer", "TestIssuer" },
                { "JwtSettings:Audience", "TestAudience" },
                { "JwtSettings:ExpiresInMinutes", "15" }
            };
            return new ConfigurationBuilder()
                .AddInMemoryCollection(inMemorySettings)
                .Build();
        }

      
        private async Task<User> CreateUserAsync()
        {
            var user = new User
            {
                PasswordHash = "somehash",
                Username = "testuser",
                Role = UserRole.Member,
            };
            await _context.Users.AddAsync(user);
            await _context.SaveChangesAsync();
            return user;
        }

        [Fact]
        public async Task Should_Return_Tokens_When_RefreshToken_Is_Valid()
        {
            var user = await CreateUserAsync();
            var refreshtoken = new RefreshToken
            {
                UserId = user.Id,
                Token = "valid-token-123",
                IsActive = true,
                ExpiresAt = DateTime.UtcNow.AddDays(30)
            };
            await _context.RefreshTokens.AddAsync(refreshtoken);
            await _context.SaveChangesAsync();

            var dto = new RefreshRequestDto { RefreshToken = "valid-token-123" };
            var result = await _authService.RefreshTokenAsync(dto);

            Assert.NotNull(result);
        }

        [Fact]
        public async Task Should_Return_Null_When_RefreshToken_Is_Inactive()
        {
            var user = await CreateUserAsync();
            var refreshtoken = new RefreshToken
            {
                UserId = user.Id,
                Token = "valid-token-123",
                IsActive = false,
                ExpiresAt = DateTime.UtcNow.AddDays(30)
            };
            await _context.RefreshTokens.AddAsync(refreshtoken);
            await _context.SaveChangesAsync();

            var dto = new RefreshRequestDto { RefreshToken = "valid-token-123" };
            var result = await _authService.RefreshTokenAsync(dto);

            Assert.Null(result);
        }

        [Fact]
        public async Task Should_Return_Null_When_RefreshToken_Is_Expired()
        {
            var user = await CreateUserAsync();
            var refreshtoken = new RefreshToken
            {
                UserId = user.Id,
                Token = "valid-token-123",
                IsActive = true,
                ExpiresAt = DateTime.UtcNow.AddDays(-1)
            };
            await _context.RefreshTokens.AddAsync(refreshtoken);
            await _context.SaveChangesAsync();

            var dto = new RefreshRequestDto { RefreshToken = "valid-token-123" };
            var result = await _authService.RefreshTokenAsync(dto);

            Assert.Null(result);
        }

        [Fact]
        public async Task Should_Revoke_Token_When_Logout_Called_With_Valid_Token()
        {
            var user = await CreateUserAsync();
            var refreshtoken = new RefreshToken {
                UserId = user.Id,
                Token = "valid-token-123",
                IsActive = true,
                ExpiresAt = DateTime.UtcNow.AddDays(30)
            };
            await _context.RefreshTokens.AddAsync(refreshtoken);
            await _context.SaveChangesAsync();
            var result = await _authService.LogoutAsync(new RefreshRequestDto { RefreshToken = "valid-token-123" });
            Assert.Equal(Result.ErrorTypes.None, result.ErrorType);
            var updatedToken = await _context.RefreshTokens.FirstOrDefaultAsync(r => r.Token == "valid-token-123");
            Assert.False(updatedToken.IsActive);


        }

        [Fact]
        public async Task Should_Revoke_All_Active_Tokens_When_Inactive_Token_Reused()
        {
            var user = await CreateUserAsync();
            var refreshtoken = new RefreshToken
            {
                UserId = user.Id,
                Token = "valid-token-123",
                IsActive = true,
                ExpiresAt = DateTime.UtcNow.AddDays(30)
            };
            await _context.RefreshTokens.AddAsync(refreshtoken);
           
            var refreshtoken2 = new RefreshToken
            {
                UserId = user.Id,
                Token = "valid-token-1234",
                IsActive = true,
                ExpiresAt = DateTime.UtcNow.AddDays(30)
            };
            await _context.RefreshTokens.AddAsync(refreshtoken2);
            var refreshtoken3 = new RefreshToken
            {
                UserId = user.Id,
                Token = "invalid-token-1234",
                IsActive = false,
                ExpiresAt = DateTime.UtcNow.AddDays(10)
            };
            await _context.RefreshTokens.AddAsync(refreshtoken3);
            await _context.SaveChangesAsync();
            var dto = new RefreshRequestDto { RefreshToken = "invalid-token-1234" };
            var result = await _authService.RefreshTokenAsync(dto);
            Assert.Null(result);
            Assert.False(refreshtoken2.IsActive);
            Assert.False(refreshtoken.IsActive);


        }
    }
}