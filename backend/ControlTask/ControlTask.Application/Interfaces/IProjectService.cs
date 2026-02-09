using ControlTask.Application.DTOs;

namespace ControlTask.Application.Interfaces
{
    public interface IProjectService
    {
        Task<IEnumerable<ProjectDto>> GetProjectsWithStatsAsync();
        Task<ProjectDto?> GetProjectByIdAsync(int id);
        Task<IEnumerable<ProjectHealthDto>> GetProjectHealthAsync();
        Task<PagedResultDto<TaskDto>> GetProjectTasksPagedAsync(int projectId, int page, int pageSize, string? status = null, int? assigneeId = null);
        Task<IEnumerable<TaskDto>> GetProjectTasksAsync(int projectId, string? status = null, int? assigneeId = null);
    }
}
