using GymManagment.Domain.DTO;

namespace GymManagment.Application.Interfaces
{
    public interface IMemberService
    {
        List<MemberDto> GetAllMembers();
        MemberDto? GetMemberById(int id);
        bool CreateMember(MemberDto dto);
        bool UpdateMember(int id, MemberDto dto);
        bool DeleteMember(int id);
    }
}
