using GymManagment.Application.Interfaces;
using GymManagment.Application.Repositories;
using GymManagment.Domain.DTO;
using GymManagment.Domain.Models;
using GymManagment.Infrastructure.Data;
using Microsoft.EntityFrameworkCore;
using TrainerEntity = GymManagment.Domain.Models.Trainer;
namespace GymManagment.Application.Repositories
{
    public class TrainerRepository:IRepository<Trainer>, ITrainerRepository
    {
        private readonly GymDbContext _context;

        public TrainerRepository(GymDbContext context) { 
        _context = context;
        }
        public async Task<List<TrainerEntity>> GetAllAsync()
        {
            return await _context.Trainers.AsNoTracking().ToListAsync();
        }

        public async Task<TrainerEntity?> GetByIdAsync(int id)
        {
            return await _context.Trainers.AsNoTracking().FirstOrDefaultAsync(t=>t.Id==id);
        }
        public async Task AddAsync(Trainer entity)
        {
            await _context.Trainers.AddAsync(entity);
        }

        public async Task UpdateAsync(Trainer entity)
        {
            _context.Trainers.Update(entity);
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
