using GymManagment.Application.Interfaces;
using GymManagment.Domain.Common;
using GymManagment.Domain.DTO;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;

namespace GymManagment.API.Controllers
{
    [ApiController]
    [Route("api/[controller]")]
    public class MembersController : ControllerBase
    {
        private readonly IMemberService _memberService;

        public MembersController(IMemberService memberService)
        {
            _memberService = memberService;
        }

        [HttpGet]
        [Authorize(Roles = "Admin")]
        public async Task<IActionResult> GetAll( [FromQuery] int? trainerId, [FromQuery] int page = 1)
        {
            var members = await _memberService.GetAllMembersAsync(page, trainerId);
            return Ok(members);
        }

        [HttpGet("{id}")]
        [Authorize(Roles = "Admin,Trainer")]
        public async Task<IActionResult> GetById(int id)
        {
            var member = await _memberService.GetMemberByIdAsync(id);
            if (member == null)
                return NotFound($"Клиент с ID {id} не найден");

            return Ok(member);
        }

        [HttpPost]
        [Authorize(Roles = "Admin")]
        public async Task<IActionResult> Create([FromBody] MemberDto dto)
        {
            if (dto == null)
                return BadRequest("Данные не переданы");

            Result result = await _memberService.CreateMemberAsync(dto);

            if (!result.Success)
            {

                if (result.ErrorType == Result.ErrorTypes.NotFound) return NotFound(result.ErrorMessage);

                if (result.ErrorType == Result.ErrorTypes.Conflict) return Conflict(result.ErrorMessage);

                if (result.ErrorType == Result.ErrorTypes.ServerError) return StatusCode(500, result.ErrorMessage);

                if (result.ErrorType== Result.ErrorTypes.ValidationError) return BadRequest(result.ErrorMessage);
            }

            return Ok();
        }

        [HttpPut("{id}")]
        [Authorize(Roles = "Admin")]
        public async Task<IActionResult> Update(int id, [FromBody] MemberDto dto)
        {
            if (dto == null)
                return BadRequest("Данные не переданы");

            Result result = await _memberService.UpdateMemberAsync(id, dto);

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
            Result result = await _memberService.DeleteMemberAsync(id);
            if (!result.Success)
            {

                if (result.ErrorType == Result.ErrorTypes.NotFound) return NotFound(result.ErrorMessage);

                if (result.ErrorType == Result.ErrorTypes.Conflict) return Conflict(result.ErrorMessage);

                if (result.ErrorType == Result.ErrorTypes.ServerError) return StatusCode(500, result.ErrorMessage);

                if (result.ErrorType == Result.ErrorTypes.ValidationError) return BadRequest(result.ErrorMessage);
            }

            

            return NoContent();
        }
      
    }
}