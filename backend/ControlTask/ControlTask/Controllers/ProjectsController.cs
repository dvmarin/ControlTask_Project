using ControlTask.Application.Interfaces;
using Microsoft.AspNetCore.Mvc;

namespace ControlTask.API.Controllers
{
    [ApiController]
    [Route("api/[controller]")]
    public class ProjectsController : ControllerBase
    {
        private readonly IProjectService _projectService;

        public ProjectsController(IProjectService projectService)
        {
            _projectService = projectService;
        }

        [HttpGet]
        public async Task<IActionResult> GetProjects()
        {
            try
            {
                var projects = await _projectService.GetProjectsWithStatsAsync();
                return Ok(projects);
            }
            catch (Exception ex)
            {
                return StatusCode(500, $"Error interno: {ex.Message}");
            }
        }

        [HttpGet("{id}")]
        public async Task<IActionResult> GetProject(int id)
        {
            try
            {
                var project = await _projectService.GetProjectByIdAsync(id);
                if (project == null)
                    return NotFound($"Proyecto con ID {id} no encontrado.");

                return Ok(project);
            }
            catch (Exception ex)
            {
                return StatusCode(500, $"Error interno: {ex.Message}");
            }
        }

        [HttpGet("{id}/tasks")]
        public async Task<IActionResult> GetProjectTasks(
            int id,
            [FromQuery] string? status = null,
            [FromQuery] int? assigneeId = null,
            [FromQuery] int page = 1,
            [FromQuery] int pageSize = 10)
        {
            try
            {
                if (page < 1)
                    return BadRequest("El número de página debe ser mayor a 0.");

                if (pageSize < 1 || pageSize > 100)
                    return BadRequest("El tamaño de página debe estar entre 1 y 100.");

                if (!string.IsNullOrEmpty(status) &&
                    !new[] { "ToDo", "InProgress", "Blocked", "Completed" }.Contains(status))
                {
                    return BadRequest("Status debe ser: ToDo, InProgress, Blocked o Completed.");
                }

                var result = await _projectService.GetProjectTasksPagedAsync(
                    id, page, pageSize, status, assigneeId);

                return Ok(result);
            }
            catch (KeyNotFoundException ex)
            {
                return NotFound(ex.Message);
            }
            catch (Exception ex)
            {
                return StatusCode(500, $"Error interno: {ex.Message}");
            }
        }

        [HttpGet("{id}/tasks/all")]
        public async Task<IActionResult> GetAllProjectTasks(
            int id,
            [FromQuery] string? status = null,
            [FromQuery] int? assigneeId = null)
        {
            try
            {
                var tasks = await _projectService.GetProjectTasksAsync(id, status, assigneeId);
                return Ok(tasks);
            }
            catch (KeyNotFoundException ex)
            {
                return NotFound(ex.Message);
            }
            catch (Exception ex)
            {
                return StatusCode(500, $"Error interno: {ex.Message}");
            }
        }

        [HttpGet("health")]
        public async Task<IActionResult> GetProjectHealth()
        {
            try
            {
                var health = await _projectService.GetProjectHealthAsync();
                return Ok(health);
            }
            catch (Exception ex)
            {
                return StatusCode(500, $"Error interno: {ex.Message}");
            }
        }
    }
}