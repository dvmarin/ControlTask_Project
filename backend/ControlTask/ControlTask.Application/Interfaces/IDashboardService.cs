using ControlTask.Application.DTOs;

namespace ControlTask.Application.Interfaces
{
    public interface IDashboardService
    {
        Task<IEnumerable<DeveloperWorkloadDto>> GetDeveloperWorkloadAsync();
        Task<IEnumerable<ProjectHealthDto>> GetProjectHealthAsync();
        Task<IEnumerable<DeveloperDelayRiskDto>> GetDeveloperDelayRiskAsync();
        Task<IEnumerable<UpcomingTaskDto>> GetUpcomingTasksAsync(int days = 7);
    }
}
