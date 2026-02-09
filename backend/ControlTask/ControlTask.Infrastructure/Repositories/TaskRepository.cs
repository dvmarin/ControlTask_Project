using ControlTask.Domain.Entities;
using ControlTask.Domain.Interfaces;
using ControlTask.Infrastructure.Persistence;
using Microsoft.EntityFrameworkCore;
using System.Linq.Expressions;

namespace ControlTask.Infrastructure.Repositories
{
    public class TaskRepository : ITaskRepository
    {
        private readonly ApplicationDbContext _context;
        private readonly DbSet<TaskItem> _dbSet;

        public TaskRepository(ApplicationDbContext context)
        {
            _context = context;
            _dbSet = context.Set<TaskItem>();
        }

        public async Task<IEnumerable<TaskItem>> GetTasksByProjectAsync(int projectId, string? status = null, int? assigneeId = null)
        {
            var query = _dbSet
                .Include(t => t.Project)
                .Include(t => t.Assignee)
                .Where(t => t.ProjectId == projectId);

            if (!string.IsNullOrEmpty(status))
            {
                query = query.Where(t => t.Status == status);
            }

            if (assigneeId.HasValue)
            {
                query = query.Where(t => t.AssigneeId == assigneeId.Value);
            }

            return await query.OrderByDescending(t => t.CreatedAt).ToListAsync();
        }

        public async Task<PagedResult<TaskItem>> GetPagedTasksByProjectAsync(int projectId, int pageNumber = 1, int pageSize = 10, string? status = null, int? assigneeId = null)
        {
            var query = _dbSet
                .Include(t => t.Project)
                .Include(t => t.Assignee)
                .Where(t => t.ProjectId == projectId);

            if (!string.IsNullOrEmpty(status))
            {
                query = query.Where(t => t.Status == status);
            }

            if (assigneeId.HasValue)
            {
                query = query.Where(t => t.AssigneeId == assigneeId.Value);
            }

            var totalCount = await query.CountAsync();
            var items = await query
                .OrderByDescending(t => t.CreatedAt)
                .Skip((pageNumber - 1) * pageSize)
                .Take(pageSize)
                .ToListAsync();

            return new PagedResult<TaskItem>
            {
                Items = items,
                TotalCount = totalCount,
                PageNumber = pageNumber,
                PageSize = pageSize
            };
        }

        public async Task<IEnumerable<TaskItem>> GetUpcomingTasksAsync(int days = 7)
        {
            var today = DateTime.UtcNow.Date;
            var targetDate = today.AddDays(days);

            return await _dbSet
                .Include(t => t.Project)
                .Include(t => t.Assignee)
                .Where(t => t.Status != "Completed" &&
                           t.DueDate.HasValue &&
                           t.DueDate >= today &&
                           t.DueDate <= targetDate)
                .OrderBy(t => t.DueDate)
                .ToListAsync();
        }

        public async Task<IEnumerable<TaskItem>> GetTasksByAssigneeAsync(int assigneeId)
        {
            return await _dbSet
                .Include(t => t.Project)
                .Include(t => t.Assignee)
                .Where(t => t.AssigneeId == assigneeId)
                .OrderByDescending(t => t.CreatedAt)
                .ToListAsync();
        }

        public async Task<IEnumerable<TaskItem>> GetTasksDueBeforeAsync(DateTime date)
        {
            return await _dbSet
                .Include(t => t.Project)
                .Include(t => t.Assignee)
                .Where(t => t.DueDate.HasValue && t.DueDate <= date)
                .OrderBy(t => t.DueDate)
                .ToListAsync();
        }

        // ✅ Métodos CRUD básicos (implementados)
        public async Task<TaskItem?> GetByIdAsync(int id)
        {
            return await _dbSet
                .Include(t => t.Project)
                .Include(t => t.Assignee)
                .FirstOrDefaultAsync(t => t.TaskId == id);
        }

        public async Task<IEnumerable<TaskItem>> GetAllAsync()
        {
            return await _dbSet
                .Include(t => t.Project)
                .Include(t => t.Assignee)
                .ToListAsync();
        }

        public async Task<IEnumerable<TaskItem>> FindAsync(Expression<Func<TaskItem, bool>> predicate)
        {
            return await _dbSet
                .Include(t => t.Project)
                .Include(t => t.Assignee)
                .Where(predicate)
                .ToListAsync();
        }

        public async Task<TaskItem> AddAsync(TaskItem entity)
        {
            await _dbSet.AddAsync(entity);
            await _context.SaveChangesAsync();
            return entity;
        }

        public async Task UpdateAsync(TaskItem entity)
        {
            entity.UpdatedAt = DateTime.UtcNow;
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
            return await _dbSet.AnyAsync(e => e.TaskId == id);
        }

        public async Task<int> CountAsync()
        {
            return await _dbSet.CountAsync();
        }

        public async Task<int> CountAsync(Expression<Func<TaskItem, bool>> predicate)
        {
            return await _dbSet.CountAsync(predicate);
        }
    }
}