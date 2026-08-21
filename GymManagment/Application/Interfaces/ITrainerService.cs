
using GymManagment.Domain.DTO;
using GymManagment.Domain.Common;

namespace GymManagment.Application.Interfaces
{
    public interface ITrainerService
    {
        Task<PagedResult<TrainerDto>> GetAllTrainersAsync(int page);
        Task<TrainerDto?> GetTrainerByIdAsync(int id);
        Task<Result> CreateTrainerAsync(TrainerDto trainerDto);
        Task<Result> UpdateTrainerAsync(int id, TrainerDto trainerDto);
        Task<Result> DeleteTrainerAsync(int id);
    }
}
