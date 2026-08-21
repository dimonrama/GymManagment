using GymManagment.Application.Interfaces;
using GymManagment.Domain.Common;
using GymManagment.Domain.Models;

namespace GymManagment.Application.Repositories
{
    public interface ITrainerRepository : IRepository<Trainer>
    {
        Task<PagedResult<Trainer>> GetAllAsync(int page);
        Task<bool> HasActiveMembersAsync(int trainerId);
        Task<Trainer?> GetByIdTrackedAsync(int id);
    }
}