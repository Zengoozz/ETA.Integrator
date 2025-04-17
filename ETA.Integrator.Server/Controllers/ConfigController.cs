using ETA.Integrator.Server.Models;
using Microsoft.AspNetCore.Mvc;
using RestSharp;
using DotNetEnv;
using ETA.Integrator.Server.Models.Provider.Requests;
using ETA.Integrator.Server.Models.Provider.Response;
using ETA.Integrator.Server.Interface.Services;
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

        private readonly IConfigurationService _configurationService;
        private readonly ISettingsStepRepository _settingsStepRepository;
        public ConfigController(
            IOptions<CustomConfigurations> customConfigurations,
            ILogger<ConfigController> logger,
            IConfigurationService configurationService,
            ISettingsStepRepository settingsStepRepository
            )
        {

            _customConfig = customConfigurations.Value;
            _logger = logger;
            _configurationService = configurationService;
            _settingsStepRepository = settingsStepRepository;

            //var connectionString = Environment.GetEnvironmentVariable("HMS_API");
            //var connectionString = Environment.GetEnvironmentVariable("HMS_PUBLISH");
            var API_URL = _customConfig.API_URL;

            if (API_URL != null)
            {
                var opt = new RestClientOptions(API_URL);
                _client = new RestClient(opt);
            }
            else
            {
                throw new Exception("Error: Getting connection string");
            }
            _logger = logger;
        }
        [HttpPost("Login")]
        public async Task<IActionResult> Login([FromBody] ProviderLoginRequestModel model)
        {
            try
            {
                var request = new RestRequest("/api/Auth/LogIn", Method.Post)
                    .AddJsonBody(model);

                var response = await _client.ExecuteAsync<ProviderLoginResponseModel>(request);

                //TODO: Check the settings 
                //var settings = _configurationService.GetSettings();

                //if (settings.ConnectionString == "" || settings.ClientId == "" || settings.ClientSecret == "")
                //{
                //    //return Redirect("/sett")
                //}

                return Ok(response);
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "An error occured getting connection settings");
                return StatusCode(500, "An unexpected error occurred getting connection settings");
            }
        }
        [HttpGet("ConnectionSettings")]
        public async Task<IActionResult> GetConnectionSettings()
        {
            try
            {
                SettingsStep step = await _settingsStepRepository.GetByStepNumber(1);

                //if (!String.IsNullOrWhiteSpace(step.Data))
                //{
                //    var test = JsonSerializer.Deserialize<ConnectionDTO>(step.Data);
                //}

                ConnectionDTO connectionDto = !String.IsNullOrWhiteSpace(step.Data) ?
                    JsonSerializer.Deserialize<ConnectionDTO>(step.Data) ?? new ConnectionDTO() :
                    new ConnectionDTO();

                return Ok(connectionDto);
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "An error occured getting connection settings");
                return StatusCode(500, "An unexpected error occurred getting connection settings");
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
                return StatusCode(500, "An unexpected error occurred getting issuer settings");
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
                return StatusCode(500, "An unexpected error occurred while getting user progress.");
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
                return StatusCode(500, "An unexpected error occurred while updating the step.");
            }
        }

        [HttpPost("SaveConnectionSettings")]
        public IActionResult SaveConnectionSettings([FromBody] ConnectionSettingsModel requestModel)
        {
            try
            {
                var connectionString = Environment.GetEnvironmentVariable("HMS_API");

                if (requestModel == null || string.IsNullOrWhiteSpace(requestModel.ConnectionString) || string.IsNullOrWhiteSpace(requestModel.ClientId) || string.IsNullOrWhiteSpace(requestModel.ClientSecret))
                {
                    return BadRequest("Empty Setting");
                }
                else
                {
                    System.IO.File.WriteAllText(".env", "HMS_API=" + requestModel.ConnectionString);
                    //Reloading Env
                    Env.Load();

                    bool configResponse = _configurationService.SetETAConfig(clientId: requestModel.ClientId, clientSecret: requestModel.ClientSecret);

                    return configResponse ? Ok("Success") : StatusCode(500, "Error: Saving ETA Config");
                }
            }
            catch (Exception)
            {
                throw;
            }
        }
    }
}
