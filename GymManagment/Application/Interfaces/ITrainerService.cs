using GymManagment.Domain.Models;
using System.ComponentModel.DataAnnotations;
using GymManagment.Domain.DTO;

namespace GymManagment.Application.Interfaces
{
    public interface ITrainerService
    {
        List<TrainerDto> GetAllTrainers();
        TrainerDto? GetTrainerById(int id);
        bool CreateTrainer(TrainerDto trainerDto);
        bool UpdateTrainer(int id, TrainerDto trainerDto);
        bool DeleteTrainer(int id);
    }
}
