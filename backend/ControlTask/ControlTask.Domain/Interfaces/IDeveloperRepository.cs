using ControlTask.Domain.Entities;

namespace ControlTask.Domain.Interfaces
{
    public interface IDeveloperRepository : IGenericRepository<Developer>
    {
        Task<IEnumerable<Developer>> GetActiveAsync();
        Task<IEnumerable<Developer>> GetDevelopersWithTasksAsync();
    }
}
