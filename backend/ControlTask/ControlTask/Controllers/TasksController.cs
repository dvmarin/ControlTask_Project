using Microsoft.AspNetCore.Mvc;
using ControlTask.Application.DTOs;
using ControlTask.Application.Interfaces;

namespace ControlTask.API.Controllers
{
    [ApiController]
    [Route("api/[controller]")]
    public class TasksController : ControllerBase
    {
        private readonly ITaskService _taskService;

        public TasksController(ITaskService taskService)
        {
            _taskService = taskService;
        }

        [HttpGet("{id}")]
        public async Task<IActionResult> GetTask(int id)
        {
            try
            {
                var task = await _taskService.GetTaskByIdAsync(id);
                if (task == null)
                    return NotFound($"Tarea con ID {id} no encontrada.");

                return Ok(task);
            }
            catch (Exception ex)
            {
                return StatusCode(500, $"Error interno: {ex.Message}");
            }
        }

        [HttpPost]
        public async Task<IActionResult> CreateTask([FromBody] CreateTaskDto createTaskDto)
        {
            try
            {
                var task = await _taskService.CreateTaskAsync(createTaskDto);
                return CreatedAtAction(nameof(GetTask), new { id = task.TaskId }, task);
            }
            catch (KeyNotFoundException ex)
            {
                return BadRequest(ex.Message);
            }
            catch (ArgumentException ex)
            {
                return BadRequest(ex.Message);
            }
            catch (Exception ex)
            {
                return StatusCode(500, $"Error interno: {ex.Message}");
            }
        }

        [HttpPut("{id}/status")]
        public async Task<IActionResult> UpdateTaskStatus(int id, [FromBody] UpdateTaskStatusDto updateDto)
        {
            try
            {
                var task = await _taskService.UpdateTaskStatusAsync(id, updateDto);
                return Ok(task);
            }
            catch (KeyNotFoundException ex)
            {
                return NotFound(ex.Message);
            }
            catch (ArgumentException ex)
            {
                return BadRequest(ex.Message);
            }
            catch (Exception ex)
            {
                return StatusCode(500, $"Error interno: {ex.Message}");
            }
        }

        [HttpDelete("{id}")]
        public async Task<IActionResult> DeleteTask(int id)
        {
            try
            {
                await _taskService.DeleteTaskAsync(id);
                return NoContent();
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

        [HttpGet("upcoming")]
        public async Task<IActionResult> GetUpcomingTasks([FromQuery] int days = 7)
        {
            try
            {
                if (days <= 0 || days > 30)
                    return BadRequest("El número de días debe estar entre 1 y 30.");

                var tasks = await _taskService.GetUpcomingTasksAsync(days);
                return Ok(tasks);
            }
            catch (Exception ex)
            {
                return StatusCode(500, $"Error interno: {ex.Message}");
            }
        }

        [HttpGet("assignee/{assigneeId}")]
        public async Task<IActionResult> GetTasksByAssignee(int assigneeId)
        {
            try
            {
                var tasks = await _taskService.GetTasksByAssigneeAsync(assigneeId);
                return Ok(tasks);
            }
            catch (Exception ex)
            {
                return StatusCode(500, $"Error interno: {ex.Message}");
            }
        }
    }
}