
using GymManagment.Domain.Common;
using GymManagment.Domain.Models;
using GymManagment.Infrastructure.Data;
using Microsoft.EntityFrameworkCore;


namespace GymManagment.Application.Repositories
{
    public class MemberRepository : IMemberRepository
    {
        private readonly GymDbContext _context;

        public MemberRepository(GymDbContext context)
        {
            _context = context;
        }

        public async Task<PagedResult<Member>> GetAllAsync(int page, int? trainerId = null)
        {
            if (page < 1) page = 1;
            const int pageSize = 10;

            var query = _context.Members.AsNoTracking().AsQueryable();

            if (trainerId.HasValue)
            {
                query = query.Where(m => m.TrainerId == trainerId.Value);
            }
            query = query.OrderBy(q=>q.FullName);
            var totalCount = await query.CountAsync();
            var items = await query
                .Skip((page - 1) * pageSize)
                .Take(pageSize)
                .ToListAsync();

            return new PagedResult<Member>
            {
                Items = items,
                TotalCount = totalCount,
                TotalPages = (int)Math.Ceiling((double)totalCount / pageSize),
                CurrentPage = page,
                PageSize = pageSize
            };
        }
        public async Task<Member?> GetByIdTrackedAsync(int id)
        {
            return await _context.Members.FirstOrDefaultAsync(x => x.Id == id);
        }
        public async Task<Member?> GetByIdAsync(int id)
        {
            return await _context.Members.AsNoTracking().FirstOrDefaultAsync(m => m.Id == id);
        }

        public async Task AddAsync(Member entity)
        {
            await _context.Members.AddAsync(entity);
        }

        public  Task UpdateAsync(Member entity)
        {
             _context.Members.Update(entity);
            return Task.CompletedTask;
        }

        public async Task DeleteAsync(int id)
        {
            var member = await _context.Members.FindAsync(id);
            if (member != null)
            {
                _context.Members.Remove(member);
            }
        }

        public async Task<bool> SaveChangesAsync()
        {
            try
            {
                int affectedRows = await _context.SaveChangesAsync();
                return affectedRows > 0;
            }
            catch (Exception ex)
            {
                Console.WriteLine(ex.Message);
                return false;
            }
        }

        public async Task<bool> IsEmailExistsAsync(string email)
        {
            return await _context.Members
                .AnyAsync(m => m.Email == email);
        }
    }
}