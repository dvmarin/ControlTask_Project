using ControlTask.Application.DTOs;

namespace ControlTask.Application.Interfaces
{
    public interface IDeveloperService
    {
        Task<IEnumerable<DeveloperDto>> GetActiveDevelopersAsync();
        Task<DeveloperDto?> GetDeveloperByIdAsync(int id);
        Task<IEnumerable<DeveloperWorkloadDto>> GetDeveloperWorkloadAsync();
    }

}