using ControlTask.Domain.Entities;

namespace ControlTask.Domain.Interfaces
{
    public interface IProjectRepository : IGenericRepository<Project>
    {
        Task<IEnumerable<Project>> GetProjectsWithTasksAsync();
        Task<Project?> GetProjectWithTasksByIdAsync(int id);
    }
}
