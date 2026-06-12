using AutoMapper;
using GymManagment.Application.Interfaces;
using GymManagment.Application.Repositories;
using GymManagment.Domain.DTO;
using GymManagment.Domain.Models;
using GymManagment.Infrastructure.Data;

namespace GymManagment.Application.Services
{
    public class MemberService : IMemberService
    {
                  
        private readonly IMapper _mapper;
        private readonly IMemberRepository _memberRepository;   

        public MemberService(
            
            IMapper mapper,
            IMemberRepository memberRepository)  
        {
            
            _mapper = mapper;
            _memberRepository = memberRepository;
        }

        public List<MemberDto> GetAllMembers()
        {
            var members = _memberRepository.GetAllAsync().Result;  
            return _mapper.Map<List<MemberDto>>(members);
        }

        public MemberDto? GetMemberById(int id)
        {
            var member = _memberRepository.GetByIdAsync(id);
            return _mapper.Map<MemberDto>(member);
        }

        public bool CreateMember(MemberDto dto)
        {
            if (dto == null || string.IsNullOrWhiteSpace(dto.FullName))
                return false;

            if (dto.Age < 14 || dto.Age > 80)
                return false;
            if (_memberRepository.IsEmailExistsAsync(dto.Email).Result)
                return false;

            var member = _mapper.Map<Member>(dto);

            _memberRepository.AddAsync(member).Wait();
            _memberRepository.SaveChangesAsync();
            return _memberRepository.SaveChangesAsync().Result;
        }

        public bool UpdateMember(int id, MemberDto dto)
        {
            if (dto == null || string.IsNullOrWhiteSpace(dto.FullName))
                return false;

            if (dto.Age < 14 || dto.Age > 80)
                return false;

            var member = _memberRepository.GetByIdAsync(id).Result;
            if (member == null)
                return false;
;

        _mapper.Map(dto,member);

            _memberRepository.UpdateAsync(member).Wait();
            return _memberRepository.SaveChangesAsync().Result;
        }

        public bool DeleteMember(int id)
        { 
            _memberRepository.DeleteAsync(id).Wait();
            return _memberRepository.SaveChangesAsync().Result;
        }
    }
}