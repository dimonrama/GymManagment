using GymManagment.Application.Interfaces;
using GymManagment.Domain.Models;

namespace GymManagment.Application.Repositories
{
    public interface ITrainerRepository : IRepository<Trainer>
    {
        Task<bool> HasActiveMembersAsync(int trainerId);
    }
}