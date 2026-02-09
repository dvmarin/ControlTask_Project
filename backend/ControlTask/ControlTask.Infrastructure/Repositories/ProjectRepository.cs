using ControlTask.Domain.Entities;
using ControlTask.Domain.Interfaces;
using ControlTask.Infrastructure.Persistence;
using Microsoft.EntityFrameworkCore;
using System.Linq.Expressions;

namespace ControlTask.Infrastructure.Repositories
{
    public class ProjectRepository : IProjectRepository
    {
        private readonly ApplicationDbContext _context;
        private readonly DbSet<Project> _dbSet;

        public ProjectRepository(ApplicationDbContext context)
        {
            _context = context;
            _dbSet = context.Set<Project>();
        }

        public async Task<IEnumerable<Project>> GetProjectsWithTasksAsync()
        {
            return await _dbSet
                .Include(p => p.Tasks)
                .ToListAsync();
        }

        public async Task<Project?> GetProjectWithTasksByIdAsync(int id)
        {
            return await _dbSet
                .Include(p => p.Tasks)
                .FirstOrDefaultAsync(p => p.ProjectId == id);
        }

        public async Task<Project?> GetByIdAsync(int id)
        {
            return await _dbSet.FindAsync(id);
        }

        public async Task<IEnumerable<Project>> GetAllAsync()
        {
            return await _dbSet.ToListAsync();
        }

        public async Task<IEnumerable<Project>> FindAsync(Expression<Func<Project, bool>> predicate)
        {
            return await _dbSet.Where(predicate).ToListAsync();
        }

        public async Task<Project> AddAsync(Project entity)
        {
            await _dbSet.AddAsync(entity);
            await _context.SaveChangesAsync();
            return entity;
        }

        public async Task UpdateAsync(Project entity)
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
            return await _dbSet.AnyAsync(e => e.ProjectId == id);
        }

        public async Task<int> CountAsync()
        {
            return await _dbSet.CountAsync();
        }

        public async Task<int> CountAsync(Expression<Func<Project, bool>> predicate)
        {
            return await _dbSet.CountAsync(predicate);
        }
    }
}