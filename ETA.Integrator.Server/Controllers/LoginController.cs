using ETA.Integrator.Server.Interface;
using ETA.Integrator.Server.Models.Requests;
using ETA.Integrator.Server.Models.Responses;
using Microsoft.AspNetCore.Mvc;
using RestSharp;

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
        public async Task<IActionResult> Login([FromBody] LoginRequestModel model)
        {
            try
            {
                var request = new RestRequest("/api/Auth/LogIn", Method.Post)
                    .AddJsonBody(model);

                var response = await _client.ExecuteAsync<LoginResponseModel>(request);

                return Ok("Hello");
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
                SettingsResponseModel model = new SettingsResponseModel();

                var connectionString = Environment.GetEnvironmentVariable("HMS_API");

                var config = _configurationService.GetETAConfig();

                model.ConnectionString = connectionString ?? "";
                model.ClientId = config?.getClientId ?? "";
                model.ClientSecret = config?.getClientSecret ?? "";

                return Ok(model);
            }
            catch (Exception)
            {
                throw;
            }
        }
    }
}
