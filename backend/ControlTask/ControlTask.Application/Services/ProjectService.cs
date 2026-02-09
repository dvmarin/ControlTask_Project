using AutoMapper;
using ControlTask.Application.DTOs;
using ControlTask.Application.Interfaces;
using ControlTask.Domain.Interfaces;

namespace ControlTask.Application.Services
{
    public class ProjectService : IProjectService
    {
        private readonly IProjectRepository _projectRepository;
        private readonly ITaskRepository _taskRepository;
        private readonly IMapper _mapper;

        public ProjectService(
            IProjectRepository projectRepository,
            ITaskRepository taskRepository,
            IMapper mapper)
        {
            _projectRepository = projectRepository;
            _taskRepository = taskRepository;
            _mapper = mapper;
        }

        public async Task<IEnumerable<ProjectDto>> GetProjectsWithStatsAsync()
        {
            var projects = await _projectRepository.GetProjectsWithTasksAsync();

            return projects.Select(p => new ProjectDto
            {
                ProjectId = p.ProjectId,
                Name = p.Name,
                ClientName = p.ClientName,
                StartDate = p.StartDate,
                EndDate = p.EndDate,
                Status = p.Status,
                TotalTasks = p.Tasks.Count,
                OpenTasks = p.Tasks.Count(t => t.Status != "Completed"),
                CompletedTasks = p.Tasks.Count(t => t.Status == "Completed")
            }).ToList();
        }

        public async Task<ProjectDto?> GetProjectByIdAsync(int id)
        {
            var project = await _projectRepository.GetProjectWithTasksByIdAsync(id);
            if (project == null)
                return null;

            return new ProjectDto
            {
                ProjectId = project.ProjectId,
                Name = project.Name,
                ClientName = project.ClientName,
                StartDate = project.StartDate,
                EndDate = project.EndDate,
                Status = project.Status,
                TotalTasks = project.Tasks.Count,
                OpenTasks = project.Tasks.Count(t => t.Status != "Completed"),
                CompletedTasks = project.Tasks.Count(t => t.Status == "Completed")
            };
        }

        public async Task<IEnumerable<ProjectHealthDto>> GetProjectHealthAsync()
        {
            var projects = await _projectRepository.GetProjectsWithTasksAsync();

            return projects.Select(p => new ProjectHealthDto
            {
                ProjectId = p.ProjectId,
                ProjectName = p.Name,
                ClientName = p.ClientName,
                TotalTasks = p.Tasks.Count,
                OpenTasks = p.Tasks.Count(t => t.Status != "Completed"),
                CompletedTasks = p.Tasks.Count(t => t.Status == "Completed")
            }).ToList();
        }

        public async Task<PagedResultDto<TaskDto>> GetProjectTasksPagedAsync(
            int projectId, int page, int pageSize, string? status = null, int? assigneeId = null)
        {
            var projectExists = await _projectRepository.ExistsAsync(projectId);
            if (!projectExists)
                throw new KeyNotFoundException($"Proyecto con ID {projectId} no encontrado.");

            var pagedResult = await _taskRepository.GetPagedTasksByProjectAsync(
                projectId, page, pageSize, status, assigneeId);

            var taskDtos = _mapper.Map<List<TaskDto>>(pagedResult.Items);

            return new PagedResultDto<TaskDto>
            {
                Items = taskDtos,
                TotalCount = pagedResult.TotalCount,
                PageNumber = page,
                PageSize = pageSize
            };
        }

        public async Task<IEnumerable<TaskDto>> GetProjectTasksAsync(
            int projectId, string? status = null, int? assigneeId = null)
        {
            var projectExists = await _projectRepository.ExistsAsync(projectId);
            if (!projectExists)
                throw new KeyNotFoundException($"Proyecto con ID {projectId} no encontrado.");

            var tasks = await _taskRepository.GetTasksByProjectAsync(projectId, status, assigneeId);
            return _mapper.Map<IEnumerable<TaskDto>>(tasks);
        }
    }
}