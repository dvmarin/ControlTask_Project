using ControlTask.Application.DTOs;

namespace ControlTask.Application.Interfaces
{
    public interface ITaskService
    {
        Task<TaskDto?> GetTaskByIdAsync(int id);
        Task<TaskDto> CreateTaskAsync(CreateTaskDto createTaskDto);
        Task<TaskDto> UpdateTaskStatusAsync(int id, UpdateTaskStatusDto updateDto);
        Task DeleteTaskAsync(int id);
        Task<PagedResultDto<TaskDto>> GetPagedTasksByProjectAsync(int projectId, int page, int pageSize, string? status = null, int? assigneeId = null);
        Task<IEnumerable<TaskDto>> GetTasksByProjectAsync(int projectId, string? status = null, int? assigneeId = null);
        Task<IEnumerable<UpcomingTaskDto>> GetUpcomingTasksAsync(int days = 7);
        Task<IEnumerable<TaskDto>> GetTasksByAssigneeAsync(int assigneeId);
    }
}
