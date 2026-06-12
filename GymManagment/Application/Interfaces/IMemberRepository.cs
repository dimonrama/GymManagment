using GymManagment.Application.Interfaces;
using GymManagment.Domain.DTO;
using GymManagment.Domain.Models;

namespace GymManagment.Application.Repositories
{
    public interface IMemberRepository : IRepository<Member>
    {
        
        
        Task <bool> IsEmailExistsAsync(string email);
    }
}