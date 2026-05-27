using GymManagment.Application.Interfaces;
using GymManagment.Domain.DTO;
using GymManagment.Infrastructure.Data;
using GymManagment.Domain.Models;

namespace GymManagment.Application.Services
{
    public class MemberService : IMemberService
    {
        private readonly GymDbContext _context;

        public MemberService(GymDbContext context)
        {
            _context = context;
        }

        public List<MemberDto> GetAllMembers()
        {
            var members = _context.Members.ToList();

            return members.Select(m => new MemberDto
            {
                Id = m.Id,
                FullName = m.FullName,
                Age = m.Age,
                Email = m.Email,
                TrainerId = m.TrainerId
            }).ToList();
        }

        public MemberDto? GetMemberById(int id)
        {
            var member = _context.Members.Find(id);
            if (member == null)
                return null;

            return new MemberDto
            {
                Id = member.Id,
                FullName = member.FullName,
                Age = member.Age,
                Email = member.Email,
                TrainerId = member.TrainerId
            };
        }

        public bool CreateMember(MemberDto dto)
        {
            if (dto == null || string.IsNullOrWhiteSpace(dto.FullName))
                return false;

            if (dto.Age < 14 || dto.Age > 80)
                return false;

            var member = new Member
            {
                FullName = dto.FullName,
                Age = dto.Age,
                Email = dto.Email,
                TrainerId = null
            };

            _context.Members.Add(member);
            _context.SaveChanges();
            return true;
        }

        public bool UpdateMember(int id, MemberDto dto)
        {
            if (dto == null || string.IsNullOrWhiteSpace(dto.FullName))
                return false;

            var member = _context.Members.Find(id);
            if (member == null)
                return false;

            if (dto.Age < 14 || dto.Age > 80)
                return false;

            member.FullName = dto.FullName;
            member.Age = dto.Age;
            member.Email = dto.Email;
            member.TrainerId = dto.TrainerId;

            _context.SaveChanges();
            return true;
        }

        public bool DeleteMember(int id)
        {
            var member = _context.Members.Find(id);
            if (member == null)
                return false;

            _context.Members.Remove(member);
            _context.SaveChanges();
            return true;
        }
    }
}