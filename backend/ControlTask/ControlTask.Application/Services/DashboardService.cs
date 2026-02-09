using ControlTask.Application.DTOs;
using ControlTask.Application.Interfaces;

namespace ControlTask.Application.Services
{
    public class DashboardService : IDashboardService
    {
        private readonly IDashboardQuery _query;

        public DashboardService(IDashboardQuery query)
        {
            _query = query;
        }

        public Task<IEnumerable<DeveloperWorkloadDto>> GetDeveloperWorkloadAsync()
            => _query.GetDeveloperWorkloadAsync();

        public Task<IEnumerable<ProjectHealthDto>> GetProjectHealthAsync()
            => _query.GetProjectHealthAsync();

        public Task<IEnumerable<DeveloperDelayRiskDto>> GetDeveloperDelayRiskAsync()
            => _query.GetDeveloperDelayRiskAsync();

        public Task<IEnumerable<UpcomingTaskDto>> GetUpcomingTasksAsync(int days = 7)
            => _query.GetUpcomingTasksAsync(days);
    }
}
