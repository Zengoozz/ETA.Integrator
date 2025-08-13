using Microsoft.AspNetCore.Mvc;
using ETA.Integrator.Server.Interface.Repositories;
using ETA.Integrator.Server.Dtos;
using ETA.Integrator.Server.Entities;
using System.Text.Json;
using ETA.Integrator.Server.Interface.Services;

namespace ETA.Integrator.Server.Controllers
{
    [Route("HMS/[controller]")]
    [ApiController]
    public class ConfigController : ControllerBase
    {

        private readonly ILogger<ConfigController> _logger;

        private readonly ISettingsStepRepository _settingsStepRepository;
        private readonly ISettingsStepService _settingsStepService;
        public ConfigController(
            ILogger<ConfigController> logger,
            ISettingsStepRepository settingsStepRepository,
            ISettingsStepService settingsStepService
            )
        {
            _logger = logger;
            _settingsStepRepository = settingsStepRepository;
            _settingsStepService = settingsStepService;
        }

        [HttpGet("ConnectionSettings")]
        public async Task<IActionResult> GetConnectionSettings()
        {
            ConnectionDTO connection = await _settingsStepService.GetConnectionData();

            return Ok(connection);
        }

        [HttpGet("IssuerSettings")]
        public async Task<IActionResult> GetIssuerSettings()
        {
            IssuerDTO? issuerDTO = await _settingsStepService.GetIssuerData();

            return Ok(issuerDTO);
        }
        [HttpGet("UserProgress")]
        public async Task<IActionResult> GetUserProgress()
        {
            // -1 : specify that all steps are completed
            // Number: specify the number of step the user need to complete
            var step = await _settingsStepRepository.GetFirstUnCompletedStepOrder();

            return Ok(step > 0 ? step : "completed");
        }
        [HttpPost("UpdateStep")]
        public async Task<IActionResult> UpdateStep([FromBody] UpdateStepDTO updateStepDTO)
        {
            if (updateStepDTO == null)
                return BadRequest("Step data is required");

            await _settingsStepRepository.UpdateStepWithData(updateStepDTO.Order, updateStepDTO.Data);

            return Ok("UPDATED");
        }

    }
}
