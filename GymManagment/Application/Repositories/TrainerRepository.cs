
using GymManagment.Domain.Common;
using GymManagment.Domain.Models;
using GymManagment.Infrastructure.Data;
using Microsoft.EntityFrameworkCore;
using TrainerEntity = GymManagment.Domain.Models.Trainer;
namespace GymManagment.Application.Repositories
{
    public class TrainerRepository :  ITrainerRepository
    {
        private readonly GymDbContext _context;

        public TrainerRepository(GymDbContext context) { 
        _context = context;
        }
       
           public async Task<PagedResult<Trainer>> GetAllAsync(int page)
        {
            if (page < 1) page = 1;
            const int pageSize = 10;
            var totalCount = await _context.Trainers.CountAsync();
            var pageResult = new PagedResult<Trainer>()
            {
                Items = await _context.Trainers
                .AsNoTracking()
                .Skip((page - 1) * pageSize)
                .Take(pageSize)
                 .ToListAsync(),
                TotalCount = totalCount,
                TotalPages = (int)Math.Ceiling((double)totalCount / pageSize),
                CurrentPage = page,
                PageSize = pageSize
            };
            return pageResult;
        }
        
        public async Task<Trainer?> GetByIdTrackedAsync (int id)
        {
           return await _context.Trainers.FirstOrDefaultAsync(t=>t.Id ==  id);
        }
        public async Task<TrainerEntity?> GetByIdAsync(int id)
        {
            return await _context.Trainers.AsNoTracking().FirstOrDefaultAsync(t=>t.Id==id);
        }
        public async Task AddAsync(Trainer entity)
        {
            await _context.Trainers.AddAsync(entity);
        }

        public  Task UpdateAsync(Trainer entity)
        {
            _context.Trainers.Update(entity);
            return Task.CompletedTask;
        }
        public async Task DeleteAsync(int id)
        {
            var trainer = await _context.Trainers.FindAsync(id);
            if (trainer != null)
            {
                _context.Trainers.Remove(trainer);
            }
        }
        public async Task<bool> SaveChangesAsync()
        {
            try
            {
                int affectedRows = await _context.SaveChangesAsync();
                return affectedRows > 0;
            }
            catch (Exception)
            {

                return false;
            }
        }
       public async Task<bool> HasActiveMembersAsync(int trainerId)
        {
            return await _context.Members.AsNoTracking()
                .AnyAsync(m => m.TrainerId == trainerId);
        }

    }
}
