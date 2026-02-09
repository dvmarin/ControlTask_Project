using ControlTask.Application.Interfaces;
using Microsoft.AspNetCore.Mvc;

namespace ControlTask.API.Controllers
{
    [ApiController]
    [Route("api/[controller]")]
    public class DevelopersController : ControllerBase
    {
        private readonly IDeveloperService _developerService;

        public DevelopersController(IDeveloperService developerService)
        {
            _developerService = developerService;
        }

        [HttpGet]
        public async Task<IActionResult> GetActiveDevelopers()
        {
            try
            {
                var developers = await _developerService.GetActiveDevelopersAsync();
                return Ok(developers);
            }
            catch (Exception ex)
            {
                return StatusCode(500, $"Error interno: {ex.Message}");
            }
        }

        [HttpGet("{id}")]
        public async Task<IActionResult> GetDeveloper(int id)
        {
            try
            {
                var developer = await _developerService.GetDeveloperByIdAsync(id);
                if (developer == null)
                    return NotFound($"Desarrollador con ID {id} no encontrado.");

                return Ok(developer);
            }
            catch (Exception ex)
            {
                return StatusCode(500, $"Error interno: {ex.Message}");
            }
        }

        [HttpGet("workload")]
        public async Task<IActionResult> GetDeveloperWorkload()
        {
            try
            {
                var workload = await _developerService.GetDeveloperWorkloadAsync();
                return Ok(workload);
            }
            catch (Exception ex)
            {
                return StatusCode(500, $"Error interno: {ex.Message}");
            }
        }
    }
}