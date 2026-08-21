using GymManagment.Application.Interfaces;
using GymManagment.Domain.Common;
using GymManagment.Domain.Models;

namespace GymManagment.Application.Repositories
{
    public interface IMemberRepository : IRepository<Member>
    {

        Task<PagedResult<Member>> GetAllAsync(int page, int? trainerId);
        Task <bool> IsEmailExistsAsync(string email);
        Task<Member?> GetByIdTrackedAsync(int id);
    }
}