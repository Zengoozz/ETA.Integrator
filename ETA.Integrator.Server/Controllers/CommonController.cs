using ETA.Integrator.Server.Interface;
using ETA.Integrator.Server.Models.ETA;
using HMS.Core.Models.ETA;
using Microsoft.AspNetCore.Mvc;
using RestSharp;
using System.Text.Json;

namespace ETA.Integrator.Server.Controllers
{
    [ApiController]
    [Route("[controller]")]
    public class CommonController : ControllerBase
    {
        private readonly IConfigurationService _ConfigurationService;

        readonly RestClient _client;
        public CommonController(
            IConfigurationService configurationService)
        {
            _ConfigurationService = configurationService;

            var opt = new RestClientOptions();
        }
        [HttpGet("Test")]
        public IActionResult Test()
        {
            var res = _ConfigurationService.SetETAConfig(generatedToken:"zengo");

            return Ok(res);
        }

        [HttpGet("Connect")]
        public async Task<IActionResult?> ConnectToETA()
        {
            var config = _ConfigurationService.GetETAConfig();

            if (config != null)
            {
                EnvironmentVariableModel? idSrvUrl = config.Values.FirstOrDefault(x => x.Key == "idSrvBaseUrl") ?? null;
                EnvironmentVariableModel? clientId = config.Values.FirstOrDefault(x => x.Key == "clientId") ?? null;
                EnvironmentVariableModel? clientSecret = config.Values.FirstOrDefault(x => x.Key == "clientSecret") ?? null;

                if (idSrvUrl != null && clientId != null && clientSecret != null)
                {
                    var connectionClient = new RestClient(idSrvUrl.Value);

                    var request = new RestRequest("/connect/token", Method.Post)
                        .AddParameter("grant_type", "client_credentials")
                        .AddParameter("client_id", clientId.Value)
                        .AddParameter("client_secret", clientSecret.Value)
                        .AddParameter("scope", "InvoicingAPI");

                    var response = await connectionClient.ExecuteAsync(request);

                    var responseObject = new ResponseModel();

                    //var client = _httpClientFactory.CreateClient();

                    //var uriStringfied = idSrvUrl.Value + "/connect/token";

                    //var formData = new Dictionary<string, string>() {
                    //    {"grant_type", "client_credentials"},
                    //    {"client_id", clientId.Value},
                    //    {"client_secret", clientSecret.Value},
                    //    {"scope", "InvoicingAPI"}
                    //};

                    //var request = new ConnectionRequestModel();

                    //request.client_id = clientId.Value;
                    //request.client_secret = clientSecret.Value;

                    //var content = new StringContent(JsonSerializer.Serialize(request), Encoding.UTF8, "application/json");

                    //var content = new FormUrlEncodedContent(formData);

                    //var response = await client.PostAsync(uriStringfied, content);
                    if (response.Content != null)
                    {
                        responseObject = JsonSerializer.Deserialize<ResponseModel>(response.Content);
                    }

                    if (responseObject != null)
                    {
                        _ConfigurationService.SetETAConfig(generatedToken: responseObject.access_token);
                        return Ok(responseObject);
                    }

                }

                return StatusCode(StatusCodes.Status500InternalServerError, "Configuration values missing");
            }
            else
            {
                return StatusCode(StatusCodes.Status500InternalServerError, "No configuration file");
            }
        }
    }
}
