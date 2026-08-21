using GymManagment.Application.Interfaces;
using GymManagment.Domain.Common;
using GymManagment.Domain.DTO;
using Microsoft.AspNetCore.Authorization;
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
        [Authorize]
        public async Task<IActionResult> GetAll([FromQuery] int page = 1)
        {
            var trainers = await _trainerService.GetAllTrainersAsync(page);
            return Ok(trainers);
        }

        [HttpGet("{id}")]
        [Authorize]
        public async Task<IActionResult> GetById(int id)
        {
            var trainer = await _trainerService.GetTrainerByIdAsync(id);
            if (trainer == null)
                return NotFound($"Тренер с ID {id} не найден");

            return Ok(trainer);
        }

        [HttpPost]
        [Authorize(Roles = "Admin")]
        public async Task<IActionResult> Create([FromBody] TrainerDto dto)
        {
            if (dto == null)
                return BadRequest("Данные тренера не переданы");

            Result result = await _trainerService.CreateTrainerAsync(dto);

            if (!result.Success)
            {

                if (result.ErrorType == Result.ErrorTypes.NotFound) return NotFound(result.ErrorMessage);

                if (result.ErrorType == Result.ErrorTypes.Conflict) return Conflict(result.ErrorMessage);

                if (result.ErrorType == Result.ErrorTypes.ServerError) return StatusCode(500, result.ErrorMessage);

                if (result.ErrorType == Result.ErrorTypes.ValidationError) return BadRequest(result.ErrorMessage);
            }

            return Ok();
        }

        [HttpPut("{id}")]
        [Authorize(Roles = "Admin")]
        public async Task<IActionResult> Update(int id, [FromBody] TrainerDto dto)
        {
            if (dto == null)
                return BadRequest("Данные не переданы");

            Result result = await _trainerService.UpdateTrainerAsync(id, dto);

            if (!result.Success)
            {

                if (result.ErrorType == Result.ErrorTypes.NotFound) return NotFound(result.ErrorMessage);

                if (result.ErrorType == Result.ErrorTypes.Conflict) return Conflict(result.ErrorMessage);

                if (result.ErrorType == Result.ErrorTypes.ServerError) return StatusCode(500, result.ErrorMessage);

                if (result.ErrorType == Result.ErrorTypes.ValidationError) return BadRequest(result.ErrorMessage);
            }

            return Ok();
        }

        [HttpDelete("{id}")]
        [Authorize(Roles = "Admin")]
        public async Task<IActionResult> Delete(int id)
        {
            Result resultOfDelete = await _trainerService.DeleteTrainerAsync(id);
            if (!resultOfDelete.Success) { 

            if (resultOfDelete.ErrorType == Result.ErrorTypes.NotFound) return NotFound(resultOfDelete.ErrorMessage);

            if (resultOfDelete.ErrorType == Result.ErrorTypes.Conflict) return Conflict(resultOfDelete.ErrorMessage);

             if (resultOfDelete.ErrorType == Result.ErrorTypes.ServerError) return StatusCode(500, resultOfDelete.ErrorMessage);

                if (resultOfDelete.ErrorType == Result.ErrorTypes.ValidationError) return BadRequest(resultOfDelete.ErrorMessage);
            }

                return NoContent(); 
        }
    }
}