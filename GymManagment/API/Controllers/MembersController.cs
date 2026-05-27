using Microsoft.AspNetCore.Mvc;
using GymManagment.Infrastructure.Data;
using GymManagment.Domain.Models;
using GymManagment.Domain.DTO;
using GymManagment.Application.Interfaces;

namespace GymManagment.API.Controllers
{
    [ApiController]
    [Route("api/[controller]")]
    public class MembersController : ControllerBase
    {
        private readonly IMemberService _memberService;
        public MembersController(IMemberService memberService) {
            _memberService = memberService;
        }

        [HttpGet]
        public IActionResult GetAll()
        {
            var members = _memberService.GetAllMembers();   

            return Ok(members);
        }

        [HttpGet("{id}")]
        public IActionResult GetById(int id)
        {
            var member = _memberService.GetMemberById(id);
            if (member == null)
                return NotFound($"Клиент с ID {id} не найден");
            return Ok(member);
        }

        [HttpPost]
        public IActionResult Create([FromBody] MemberDto dto)
        {
            if (dto == null)
                return BadRequest("Данные не переданы");

            bool success = _memberService.CreateMember(dto);

            if (!success)
                return BadRequest("Не удалось создать клиента");

            return Ok("Клиент успешно создан");
        }

        [HttpPut("{id}")]
        public IActionResult Update(int id, [FromBody] MemberDto dto)
        {
            if (dto == null)
                return BadRequest("Данные не переданы");

            bool success = _memberService.UpdateMember(id,dto);

            if (!success)
                return BadRequest("Не удалось обновить клиента");

            return Ok("Клиент успешно обновлен");
        }

        [HttpDelete("{id}")]
        public IActionResult Delete(int id)
        {
            bool success = _memberService.DeleteMember(id);
            if (!success)
                return BadRequest("Не удалось удалить клиента");
            return NoContent();
        }
    }
}