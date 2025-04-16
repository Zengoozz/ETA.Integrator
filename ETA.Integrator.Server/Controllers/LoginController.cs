using ETA.Integrator.Server.Models;
using Microsoft.AspNetCore.Mvc;
using RestSharp;
using DotNetEnv;
using ETA.Integrator.Server.Models.Provider.Requests;
using ETA.Integrator.Server.Models.Provider.Response;
using ETA.Integrator.Server.Interface.Services;

namespace ETA.Integrator.Server.Controllers
{
    [Route("HMS/[controller]")]
    [ApiController]
    public class LoginController : ControllerBase
    {
        readonly RestClient _client;
        private readonly IConfigurationService _configurationService;
        public LoginController(IConfigurationService configurationService)
        {
            _configurationService = configurationService;

            var connectionString = Environment.GetEnvironmentVariable("HMS_API");
            //var connectionString = Environment.GetEnvironmentVariable("HMS_PUBLISH");

            if (connectionString != null)
            {
                var opt = new RestClientOptions(connectionString);
                _client = new RestClient(opt);
            }
            else
            {
                throw new Exception("Error: Getting connection string");
            }
        }
        [HttpPost]
        public async Task<IActionResult> Login([FromBody] ProviderLoginRequestModel model)
        {
            try
            {
                var request = new RestRequest("/api/Auth/LogIn", Method.Post)
                    .AddJsonBody(model);

                var response = await _client.ExecuteAsync<ProviderLoginResponseModel>(request);

                //TODO: Check the settings 
                var settings = _configurationService.GetSettings();

                if(settings.ConnectionString == "" || settings.ClientId == "" || settings.ClientSecret == "")
                {
                    //return Redirect("/sett")
                }

                return Ok(response);
            }
            catch (Exception)
            {
                throw;
            }
        }
        [HttpGet("Settings")]
        public IActionResult GetSettings()
        {
            try
            {
                var response = _configurationService.GetSettings();

                return Ok(response);
            }
            catch (Exception)
            {
                throw;
            }
        }
        [HttpPost("SaveSettings")]
        public IActionResult SaveSettings([FromBody] SettingsModel requestModel)
        {
            try
            {
                var connectionString = Environment.GetEnvironmentVariable("HMS_API");

                if(requestModel == null || string.IsNullOrWhiteSpace(requestModel.ConnectionString) || string.IsNullOrWhiteSpace(requestModel.ClientId) || string.IsNullOrWhiteSpace(requestModel.ClientSecret))
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
