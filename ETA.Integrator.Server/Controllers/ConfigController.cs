﻿using Microsoft.AspNetCore.Mvc;
using RestSharp;
using ETA.Integrator.Server.Models.Provider.Requests;
using ETA.Integrator.Server.Models.Provider.Response;
using ETA.Integrator.Server.Interface.Repositories;
using ETA.Integrator.Server.Dtos;
using ETA.Integrator.Server.Entities;
using System.Text.Json;
using Microsoft.Extensions.Options;

namespace ETA.Integrator.Server.Controllers
{
    [Route("HMS/[controller]")]
    [ApiController]
    public class ConfigController : ControllerBase
    {
        readonly RestClient _client;
        private readonly CustomConfigurations _customConfig;

        private readonly ILogger<ConfigController> _logger;

        private readonly ISettingsStepRepository _settingsStepRepository;
        public ConfigController(
            IOptions<CustomConfigurations> customConfigurations,
            ILogger<ConfigController> logger,
            ISettingsStepRepository settingsStepRepository
            )
        {

            _customConfig = customConfigurations.Value;
            _logger = logger;
            _settingsStepRepository = settingsStepRepository;

            if (_customConfig.Provider_APIURL != null)
            {
                var opt = new RestClientOptions(_customConfig.Provider_APIURL);
                _client = new RestClient(opt);
            }
            else
            {
                throw new Exception("Error: Getting provider api url");
            }
        }
        [HttpPost("Login")]
        public async Task<IActionResult> Login([FromBody] ProviderLoginRequestModel model)
        {
            try
            {
                var request = new RestRequest("/api/Auth/LogIn", Method.Post)
                    .AddJsonBody(model);

                var response = await _client.ExecuteAsync<ProviderLoginResponseModel>(request);

                return Ok(response);
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "An error occured getting connection settings");
                return StatusCode(StatusCodes.Status500InternalServerError, "An unexpected error occurred getting connection settings");
            }
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
                SettingsStep step = await _settingsStepRepository.GetByStepNumber(2);

                IssuerDTO issuerDto = !String.IsNullOrWhiteSpace(step.Data) ? JsonSerializer.Deserialize<IssuerDTO>(step.Data) ?? new IssuerDTO() : new IssuerDTO();

                return Ok(issuerDto);
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
