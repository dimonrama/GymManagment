using GymManagment.Domain.Common;

namespace GymManagment.Application.Interfaces
{
    public interface IRepository<T> where T : class
    {
       
        Task<T?> GetByIdAsync(int id);
        Task AddAsync(T entity);
        Task UpdateAsync(T entity);
        Task DeleteAsync(int id);
        Task<bool> SaveChangesAsync();
    }
}