using ControlTask.Application.DTOs;
using ControlTask.Application.Interfaces;
using Microsoft.AspNetCore.Mvc;

namespace ControlTask.API.Controllers
{
    [Route("api/dashboard")]
    public class DashboardController : Controller
    {
        private readonly IDashboardService _dashboardService;

        public DashboardController(IDashboardService dashboardService)
        {
            _dashboardService = dashboardService;
        }

        [HttpGet("developer-workload")]
        [ProducesResponseType(typeof(IEnumerable<DeveloperWorkloadDto>), StatusCodes.Status200OK)]
        public async Task<IActionResult> GetDeveloperWorkload()
        {
            try
            {
                var result = await _dashboardService.GetDeveloperWorkloadAsync();
                return Ok(result);
            }
            catch (Exception ex)
            {
                return StatusCode(500, $"Error interno del servidor: {ex.Message}");
            }
        }

        /// <summary>
        /// Obtiene el resumen de estado por proyecto
        /// </summary>
        /// <returns>Lista de proyectos con estadísticas de tareas</returns>
        [HttpGet("project-health")]
        [ProducesResponseType(typeof(IEnumerable<ProjectHealthDto>), StatusCodes.Status200OK)]
        public async Task<IActionResult> GetProjectHealth()
        {
            try
            {
                var result = await _dashboardService.GetProjectHealthAsync();
                return Ok(result);
            }
            catch (Exception ex)
            {
                return StatusCode(500, $"Error interno del servidor: {ex.Message}");
            }
        }

        /// <summary>
        /// Obtiene la predicción de riesgo de retraso por desarrollador
        /// </summary>
        /// <returns>Lista de desarrolladores con análisis de riesgo</returns>
        [HttpGet("developer-delay-risk")]
        [ProducesResponseType(typeof(IEnumerable<DeveloperDelayRiskDto>), StatusCodes.Status200OK)]
        public async Task<IActionResult> GetDeveloperDelayRisk()
        {
            try
            {
                var result = await _dashboardService.GetDeveloperDelayRiskAsync();
                return Ok(result);
            }
            catch (Exception ex)
            {
                return StatusCode(500, $"Error interno del servidor: {ex.Message}");
            }
        }

        /// <summary>
        /// Obtiene tareas próximas a vencer
        /// </summary>
        /// <param name="days">Número de días a considerar (default: 7)</param>
        /// <returns>Lista de tareas próximas a vencer</returns>
        [HttpGet("upcoming-tasks")]
        [ProducesResponseType(typeof(IEnumerable<UpcomingTaskDto>), StatusCodes.Status200OK)]
        public async Task<IActionResult> GetUpcomingTasks([FromQuery] int days = 7)
        {
            try
            {
                if (days <= 0 || days > 30)
                    return BadRequest("El número de días debe estar entre 1 y 30.");

                var result = await _dashboardService.GetUpcomingTasksAsync(days);
                return Ok(result);
            }
            catch (Exception ex)
            {
                return StatusCode(500, $"Error interno del servidor: {ex.Message}");
            }
        }
    }
}
