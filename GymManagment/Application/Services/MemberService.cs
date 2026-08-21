using AutoMapper;
using GymManagment.Application.Interfaces;
using GymManagment.Application.Repositories;
using GymManagment.Domain.Common;
using GymManagment.Domain.DTO;
using GymManagment.Domain.Models;



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

        public async Task<PagedResult<MemberDto>> GetAllMembersAsync(int page, int? trainerId)
        {
            var pagedMembers = await _memberRepository.GetAllAsync(page, trainerId);
            return _mapper.Map<PagedResult<MemberDto>>(pagedMembers);
        }
       

        public async Task<MemberDto?> GetMemberByIdAsync(int id)
        {
            var member = await _memberRepository.GetByIdAsync(id);
            if (member == null)
                return null;

            return _mapper.Map<MemberDto>(member);
        }

        public async Task<Result> CreateMemberAsync(MemberDto dto)
        {
            if (dto == null || string.IsNullOrWhiteSpace(dto.FullName))
                return Result.Fail("Переданы пустые значения", Result.ErrorTypes.ValidationError) ;

            if (dto.Age < 14 || dto.Age > 80)
                return Result.Fail("Передан неправильный возраст", Result.ErrorTypes.ValidationError);
            if (await _memberRepository.IsEmailExistsAsync(dto.Email))
                return Result.Fail("Такая почта уже зарегистрированна", Result.ErrorTypes.Conflict);

            var member =  _mapper.Map<Member>(dto);

            await _memberRepository.AddAsync(member);

            var succes = await _memberRepository.SaveChangesAsync();
            if (succes == false)
            {
                return Result.Fail("Не удалось создать клиента", Result.ErrorTypes.ServerError);
            }
            return Result.Ok();
        }

        public async Task<Result> UpdateMemberAsync(int id, MemberDto dto)
        {
            if (dto == null || string.IsNullOrWhiteSpace(dto.FullName))
                return Result.Fail("Переданы пустые значения", Result.ErrorTypes.ValidationError);

            if (dto.Age < 14 || dto.Age > 80)
                return Result.Fail("Передан неправильный возраст", Result.ErrorTypes.ValidationError);

            var member = await _memberRepository.GetByIdTrackedAsync(id);
            if (member == null)
                return Result.Fail("Клиент не найден", Result.ErrorTypes.NotFound);

            _mapper.Map(dto,member);


            var succes = await _memberRepository.SaveChangesAsync();
            if (succes == false)
            {
                return Result.Fail("Не удалось обновить клиента", Result.ErrorTypes.ServerError);
            }
           
            return Result.Ok();
        }

        public async  Task<Result> DeleteMemberAsync(int id)
        {
            var member = await _memberRepository.GetByIdAsync(id);
            if (member == null)
            {
                return Result.Fail($"Клиент с ID:{id} не найден", Result.ErrorTypes.NotFound);
            }
           
            await _memberRepository.DeleteAsync(id);
            var succes = await _memberRepository.SaveChangesAsync();
            if (succes == false)
            {
                return Result.Fail("Не удалось удалить клиента", Result.ErrorTypes.ServerError);
            }
            return Result.Ok();
        }
    }
}