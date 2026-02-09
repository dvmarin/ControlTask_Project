using ControlTask.Domain.Entities;

namespace ControlTask.Domain.Interfaces
{
    public interface ITaskRepository : IGenericRepository<TaskItem>
    {
        Task<IEnumerable<TaskItem>> GetTasksByProjectAsync(int projectId, string? status = null, int? assigneeId = null);
        Task<PagedResult<TaskItem>> GetPagedTasksByProjectAsync(int projectId, int pageNumber = 1, int pageSize = 10, string? status = null, int? assigneeId = null);
        Task<IEnumerable<TaskItem>> GetUpcomingTasksAsync(int days = 7);
        Task<IEnumerable<TaskItem>> GetTasksByAssigneeAsync(int assigneeId);
        Task<IEnumerable<TaskItem>> GetTasksDueBeforeAsync(DateTime date);
    }

    public class PagedResult<T>
    {
        public List<T> Items { get; set; } = new();
        public int TotalCount { get; set; }
        public int PageNumber { get; set; }
        public int PageSize { get; set; }
        public int TotalPages => PageSize > 0 ? (int)Math.Ceiling(TotalCount / (double)PageSize) : 0;
        public bool HasPrevious => PageNumber > 1;
        public bool HasNext => PageNumber < TotalPages;
    }
}