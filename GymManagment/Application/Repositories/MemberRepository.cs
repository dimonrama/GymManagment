using GymManagment.Application.Interfaces;
using GymManagment.Application.Repositories;
using GymManagment.Domain.DTO;
using GymManagment.Domain.Models;
using GymManagment.Infrastructure.Data;
using Microsoft.EntityFrameworkCore;
using MemberEntity = GymManagment.Domain.Models.Member;

namespace GymManagment.Application.Repositories

{
    public class MemberRepository : IRepository<Member>, IMemberRepository
    {
        private readonly GymDbContext _context;
        public MemberRepository(GymDbContext context)
        {
            _context = context;
        }

        public async Task<List<MemberEntity>> GetAllAsync()
        {
            return await _context.Members.ToListAsync();
        }

        public async Task<MemberEntity?> GetByIdAsync(int id)
        {
            return await _context.Members.FindAsync(id);
        }
        public async Task AddAsync(Member entity)
        {
            await _context.Members.AddAsync(entity);
        }

        public async Task UpdateAsync(Member entity)
        {
            _context.Members.Update(entity);
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
            catch (Exception)
            {

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
