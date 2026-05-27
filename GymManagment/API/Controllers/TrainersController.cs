using GymManagment.Application.Interfaces;
using GymManagment.Domain.DTO;
using GymManagment.Domain.Models;
using GymManagment.Infrastructure.Data;
using Microsoft.AspNetCore.Mvc;
namespace GymManagment.API.Controllers
{
    [ApiController]
    [Route("api/[controller]")]

    public class TrainersController : ControllerBase
    {
    
        private readonly ITrainerService _trainerService;
        public TrainersController(ITrainerService trainerService)
        {
            _trainerService = trainerService;
        }
        
        [HttpGet]
        public IActionResult GetAll() {
            var allTrainers = _trainerService.GetAllTrainers();
            
            return Ok(allTrainers);
        }

        [HttpGet("{id}")]
        public IActionResult GetById(int id) {
           
            var trainer = _trainerService.GetTrainerById(id);
            if (trainer == null) { return NotFound($"Тренер с id {id} не найден"); }
            return Ok(trainer);

        }

        [HttpPost]

        public IActionResult Create([FromBody] TrainerDto dto) {
            if (dto == null) { return BadRequest("Переданы пустые значения!"); }
            bool success = _trainerService.CreateTrainer(dto);
            if (!success) { return BadRequest("Не удалось создать тренера"); }
            return Ok("Тренер успешно создан");
        }

        [HttpPut("{id}")]

        public IActionResult Update(int id, [FromBody] TrainerDto dto) {
            if (dto == null)
            {
                return BadRequest("Данные не переданы");
            }

            bool scs = _trainerService.UpdateTrainer(id, dto);
            if (!scs) { return BadRequest("Не удалось обновить тренера"); };
            return Ok("Тренер успешно обновлен");
        }

        [HttpDelete("{id}")]

        public IActionResult Delete(int id) { 
        bool success = _trainerService.DeleteTrainer(id);
            if (!success) { return BadRequest("Не удалось удалить тренера"); }
            return NoContent();
        }

    }
}
