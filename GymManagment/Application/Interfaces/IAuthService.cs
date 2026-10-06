using GymManagment.Domain.Common;
using GymManagment.Domain.DTO;
using GymManagment.Domain.Models;


namespace GymManagment.Application.Interfaces
{
    public interface IAuthService
    {
        Task<Result> RegisterAsync(RegisterDto dto);
        Task<TokenResponseDto?> LoginAsync(LoginDto dto);
        Task<TokenResponseDto?> RefreshTokenAsync(RefreshRequestDto dto);
        Task<Result> LogoutAsync(RefreshRequestDto dto);

        Task<Result> LinkTelegramAsync(string username, string password, long chatId);
        Task<User?> GetUserByTelegramChatIdAsync(long chatId);
    }
}
