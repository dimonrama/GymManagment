using GymManagment.Domain.Common;
using GymManagment.Domain.DTO;
using System.Threading.Tasks;

namespace GymManagment.Application.Interfaces
{
    public interface IMemberService
    {
        Task<PagedResult<MemberDto>> GetAllMembersAsync(int page, int? trainerId);
        Task<MemberDto?> GetMemberByIdAsync(int id);
         Task<Result> CreateMemberAsync(MemberDto dto);
        Task<Result> UpdateMemberAsync(int id, MemberDto dto);
        Task<Result> DeleteMemberAsync(int id);
    }
}
