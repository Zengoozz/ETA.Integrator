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
            try
            {
                SettingsStep step = await _settingsStepRepository.GetByStepNumber(1);

                ConnectionDTO connectionDto = !String.IsNullOrWhiteSpace(step.Data) ?
                    JsonSerializer.Deserialize<ConnectionDTO>(step.Data) ?? new ConnectionDTO() :
                    new ConnectionDTO();

                return Ok(connectionDto);
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "An error occured getting connection settings");
                return StatusCode(StatusCodes.Status500InternalServerError, "An unexpected error occurred getting connection settings");
            }
        }

        [HttpGet("IssuerSettings")]
        public async Task<IActionResult> GetIssuerSettings()
        {
            try
            {
                IssuerDTO? issuerDTO = await _settingsStepService.GetIssuerData();

                return Ok(issuerDTO);
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "An error occured getting issuer settings");
                return StatusCode(StatusCodes.Status500InternalServerError, "An unexpected error occurred getting issuer settings");
            }
        }
        [HttpGet("UserProgress")]
        public async Task<IActionResult> GetUserProgress()
        {
            try
            {
                // -1 : specify that all steps are completed
                // Number: specify the number of step the user need to complete
                var step = await _settingsStepRepository.GetFirstUnCompletedStepOrder();

                return Ok(step > 0 ? step : "completed");
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "An error occured getting user progress");
                return StatusCode(StatusCodes.Status500InternalServerError, "An unexpected error occurred while getting user progress.");
            }

        }
        [HttpPost("UpdateStep")]
        public async Task<IActionResult> UpdateStep([FromBody] UpdateStepDTO updateStepDTO)
        {
            if (updateStepDTO == null)
                return BadRequest("Step data is required");
            try
            {
                await _settingsStepRepository.UpdateStepWithData(updateStepDTO.Order, updateStepDTO.Data);
                return Ok("Updated");
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "An error occured updating the step with number: {Order}", updateStepDTO.Order);
                return StatusCode(StatusCodes.Status500InternalServerError, "An unexpected error occurred while updating the step.");
            }
        }

    }
}
