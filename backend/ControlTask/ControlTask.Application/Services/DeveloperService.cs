using AutoMapper;
using ControlTask.Application.DTOs;
using ControlTask.Application.Interfaces;
using ControlTask.Domain.Entities;
using ControlTask.Domain.Interfaces;

namespace ControlTask.Application.Services
{
    public class DeveloperService : IDeveloperService
    {
        private readonly IDeveloperRepository _developerRepository;
        private readonly IMapper _mapper;

        public DeveloperService(IDeveloperRepository developerRepository, IMapper mapper)
        {
            _developerRepository = developerRepository;
            _mapper = mapper;
        }

        public async Task<IEnumerable<DeveloperDto>> GetActiveDevelopersAsync()
        {
            var developers = await _developerRepository.GetActiveAsync();
            return _mapper.Map<IEnumerable<DeveloperDto>>(developers);
        }

        public async Task<DeveloperDto?> GetDeveloperByIdAsync(int id)
        {
            var developer = await _developerRepository.GetByIdAsync(id);
            if (developer == null || !developer.IsActive)
                return null;

            return _mapper.Map<DeveloperDto>(developer);
        }

        public async Task<IEnumerable<DeveloperWorkloadDto>> GetDeveloperWorkloadAsync()
        {
            var developers = await _developerRepository.GetDevelopersWithTasksAsync();

            return developers.Select(d => new DeveloperWorkloadDto
            {
                DeveloperName = $"{d.FirstName} {d.LastName}",
                OpenTasksCount = d.Tasks.Count(t => t.Status != "Completed"),
                AverageEstimatedComplexity = CalculateAverageComplexity(d.Tasks)
            }).ToList();
        }

        private decimal CalculateAverageComplexity(ICollection<TaskItem> tasks)
        {
            var openTasksWithComplexity = tasks
                .Where(t => t.Status != "Completed" && t.EstimatedComplexity.HasValue)
                .ToList();

            if (openTasksWithComplexity.Count == 0)
                return 0;

            return (decimal)openTasksWithComplexity.Average(t => t.EstimatedComplexity!.Value);
        }
    }
}