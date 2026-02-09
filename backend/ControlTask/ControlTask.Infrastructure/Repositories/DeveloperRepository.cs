using ControlTask.Domain.Entities;
using ControlTask.Domain.Interfaces;
using ControlTask.Infrastructure.Persistence;
using Microsoft.EntityFrameworkCore;
using System.Linq.Expressions;

namespace ControlTask.Infrastructure.Repositories
{
    public class DeveloperRepository : IDeveloperRepository
    {
        private readonly ApplicationDbContext _context;
        protected readonly DbSet<Developer> _dbSet;

        public DeveloperRepository(ApplicationDbContext context)
        {
            _context = context;
            _dbSet = context.Set<Developer>();
        }

        public async Task<IEnumerable<Developer>> GetActiveAsync()
        {
            return await _dbSet
                .Where(d => d.IsActive)
                .ToListAsync();
        }

        public async Task<IEnumerable<Developer>> GetDevelopersWithTasksAsync()
        {
            return await _dbSet
                .Include(d => d.Tasks)  // Incluye las tareas relacionadas
                .Where(d => d.IsActive)
                .ToListAsync();
        }
        public async Task<Developer?> GetByIdAsync(int id)
        {
            return await _dbSet.FindAsync(id);
        }

        public async Task<IEnumerable<Developer>> GetAllAsync()
        {
            return await _dbSet.ToListAsync();
        }

        public async Task<IEnumerable<Developer>> FindAsync(Expression<Func<Developer, bool>> predicate)
        {
            return await _dbSet.Where(predicate).ToListAsync();
        }

        public async Task<Developer> AddAsync(Developer entity)
        {
            await _dbSet.AddAsync(entity);
            await _context.SaveChangesAsync();
            return entity;
        }

        public async Task UpdateAsync(Developer entity)
        {
            _dbSet.Update(entity);
            await _context.SaveChangesAsync();
        }

        public async Task DeleteAsync(int id)
        {
            var entity = await GetByIdAsync(id);
            if (entity != null)
            {
                _dbSet.Remove(entity);
                await _context.SaveChangesAsync();
            }
        }

        public async Task<bool> ExistsAsync(int id)
        {
            return await _dbSet.AnyAsync(e => e.DeveloperId == id);
        }

        public async Task<int> CountAsync()
        {
            return await _dbSet.CountAsync();
        }

        public async Task<int> CountAsync(Expression<Func<Developer, bool>> predicate)
        {
            return await _dbSet.CountAsync(predicate);
        }
    }
}