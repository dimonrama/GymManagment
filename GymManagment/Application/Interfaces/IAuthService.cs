using GymManagment.Domain.Common;
using GymManagment.Domain.DTO;

namespace GymManagment.Application.Interfaces
{
    public interface IAuthService
    {
        Task<Result> RegisterAsync(RegisterDto dto);
        Task<TokenResponseDto?> LoginAsync(LoginDto dto);
        Task<TokenResponseDto?> RefreshTokenAsync(RefreshRequestDto dto);
        Task<Result> LogoutAsync(RefreshRequestDto dto);
    }
}
