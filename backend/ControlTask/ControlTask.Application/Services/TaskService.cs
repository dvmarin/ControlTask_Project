using AutoMapper;
using ControlTask.Application.DTOs;
using ControlTask.Application.Interfaces;
using ControlTask.Domain.Entities;
using ControlTask.Domain.Interfaces;

namespace ControlTask.Application.Services
{
    public class TaskService : ITaskService
    {
        private readonly ITaskRepository _taskRepository;
        private readonly IProjectRepository _projectRepository;
        private readonly IDeveloperRepository _developerRepository;
        private readonly IMapper _mapper;

        public TaskService(
            ITaskRepository taskRepository,
            IProjectRepository projectRepository,
            IDeveloperRepository developerRepository,
            IMapper mapper)
        {
            _taskRepository = taskRepository;
            _projectRepository = projectRepository;
            _developerRepository = developerRepository;
            _mapper = mapper;
        }

        public async Task<TaskDto?> GetTaskByIdAsync(int id)
        {
            var task = await _taskRepository.GetByIdAsync(id);
            if (task == null)
                return null;

            return _mapper.Map<TaskDto>(task);
        }

        public async Task<TaskDto> CreateTaskAsync(CreateTaskDto createTaskDto)
        {
            // Validar que el proyecto existe
            var projectExists = await _projectRepository.ExistsAsync(createTaskDto.ProjectId);
            if (!projectExists)
                throw new KeyNotFoundException($"Proyecto con ID {createTaskDto.ProjectId} no encontrado.");

            // Validar que el desarrollador existe y está activo
            var developer = await _developerRepository.GetByIdAsync(createTaskDto.AssigneeId);
            if (developer == null || !developer.IsActive)
                throw new ArgumentException($"Desarrollador con ID {createTaskDto.AssigneeId} no existe o no está activo.");

            // Validar status
            if (!new[] { "ToDo", "InProgress", "Blocked", "Completed" }.Contains(createTaskDto.Status))
                throw new ArgumentException("Status debe ser: ToDo, InProgress, Blocked o Completed.");

            // Validar priority
            if (!new[] { "Low", "Medium", "High" }.Contains(createTaskDto.Priority))
                throw new ArgumentException("Priority debe ser: Low, Medium o High.");

            // Validar EstimatedComplexity
            if (createTaskDto.EstimatedComplexity.HasValue &&
                (createTaskDto.EstimatedComplexity < 1 || createTaskDto.EstimatedComplexity > 5))
            {
                throw new ArgumentException("EstimatedComplexity debe estar entre 1 y 5.");
            }

            // Validar DueDate
            if (createTaskDto.DueDate.HasValue && createTaskDto.DueDate.Value < DateTime.UtcNow)
                throw new ArgumentException("DueDate no puede ser una fecha pasada.");

            var task = _mapper.Map<TaskItem>(createTaskDto);
            var createdTask = await _taskRepository.AddAsync(task);

            return _mapper.Map<TaskDto>(createdTask);
        }

        public async Task<TaskDto> UpdateTaskStatusAsync(int id, UpdateTaskStatusDto updateDto)
        {
            var task = await _taskRepository.GetByIdAsync(id);
            if (task == null)
                throw new KeyNotFoundException($"Tarea con ID {id} no encontrada.");

            // Validar status si se proporciona
            if (!string.IsNullOrEmpty(updateDto.Status) &&
                !new[] { "ToDo", "InProgress", "Blocked", "Completed" }.Contains(updateDto.Status))
            {
                throw new ArgumentException("Status debe ser: ToDo, InProgress, Blocked o Completed.");
            }

            // Validar priority si se proporciona
            if (!string.IsNullOrEmpty(updateDto.Priority) &&
                !new[] { "Low", "Medium", "High" }.Contains(updateDto.Priority))
            {
                throw new ArgumentException("Priority debe ser: Low, Medium o High.");
            }

            // Validar EstimatedComplexity si se proporciona
            if (updateDto.EstimatedComplexity.HasValue &&
                (updateDto.EstimatedComplexity < 1 || updateDto.EstimatedComplexity > 5))
            {
                throw new ArgumentException("EstimatedComplexity debe estar entre 1 y 5.");
            }

            // Actualizar campos
            if (!string.IsNullOrEmpty(updateDto.Status))
                task.Status = updateDto.Status;

            if (!string.IsNullOrEmpty(updateDto.Priority))
                task.Priority = updateDto.Priority;

            if (updateDto.EstimatedComplexity.HasValue)
                task.EstimatedComplexity = updateDto.EstimatedComplexity.Value;

            // Lógica de CompletionDate
            if (updateDto.Status == "Completed" && task.Status != "Completed")
            {
                task.CompletionDate = DateTime.UtcNow;
            }
            else if (updateDto.Status != "Completed" && task.Status == "Completed")
            {
                task.CompletionDate = null;
            }

            task.UpdatedAt = DateTime.UtcNow;
            await _taskRepository.UpdateAsync(task);

            return _mapper.Map<TaskDto>(task);
        }

        public async Task DeleteTaskAsync(int id)
        {
            var taskExists = await _taskRepository.ExistsAsync(id);
            if (!taskExists)
                throw new KeyNotFoundException($"Tarea con ID {id} no encontrada.");

            await _taskRepository.DeleteAsync(id);
        }

        public async Task<PagedResultDto<TaskDto>> GetPagedTasksByProjectAsync(
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

        public async Task<IEnumerable<TaskDto>> GetTasksByProjectAsync(
            int projectId, string? status = null, int? assigneeId = null)
        {
            var projectExists = await _projectRepository.ExistsAsync(projectId);
            if (!projectExists)
                throw new KeyNotFoundException($"Proyecto con ID {projectId} no encontrado.");

            var tasks = await _taskRepository.GetTasksByProjectAsync(projectId, status, assigneeId);
            return _mapper.Map<IEnumerable<TaskDto>>(tasks);
        }

        public async Task<IEnumerable<UpcomingTaskDto>> GetUpcomingTasksAsync(int days = 7)
        {
            var tasks = await _taskRepository.GetUpcomingTasksAsync(days);
            var today = DateTime.UtcNow.Date;

            return tasks.Select(t => new UpcomingTaskDto
            {
                Title = t.Title,
                ProjectName = t.Project.Name,
                AssignedTo = $"{t.Assignee.FirstName} {t.Assignee.LastName}",
                Status = t.Status,
                Priority = t.Priority,
                DueDate = t.DueDate!.Value,
                DaysUntilDue = (t.DueDate.Value.Date - today).Days
            }).ToList();
        }

        public async Task<IEnumerable<TaskDto>> GetTasksByAssigneeAsync(int assigneeId)
        {
            var tasks = await _taskRepository.GetTasksByAssigneeAsync(assigneeId);
            return _mapper.Map<IEnumerable<TaskDto>>(tasks);
        }
    }
}