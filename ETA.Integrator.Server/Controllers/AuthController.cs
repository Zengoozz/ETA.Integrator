using ETA.Integrator.Server.Models;
using ETA.Integrator.Server.Models.Consumer.Response;
using ETA.Integrator.Server.Models.Provider.Requests;
using ETA.Integrator.Server.Models.Provider.Response;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Extensions.Options;
using RestSharp;
using System.Text.Json;

namespace ETA.Integrator.Server.Controllers
{
    [ApiController]
    [Route("HMS/[controller]")]
    public class AuthController : ControllerBase
    {
        private readonly CustomConfigurations _customConfig;
        private readonly ILogger<AuthController> _logger;

        public AuthController(
            IOptions<CustomConfigurations> customConfig,
            ILogger<AuthController> logger
            )
        {
            _customConfig = customConfig.Value;
            _logger = logger;


        }

        [HttpPost("ProviderConnect")]
        public async Task<IActionResult> ConnectToProvider([FromBody] ProviderLoginRequestModel model)
        {
            try
            {

                if (_customConfig.Provider_APIURL != null)
                {
                    var opt = new RestClientOptions(_customConfig.Provider_APIURL);

                    var connectionClient = new RestClient(opt);

                    var request = new RestRequest("/api/Auth/LogIn", Method.Post)
                    .AddJsonBody(model);
                    var responseTemp = new ProviderLoginResponseModel();
                    responseTemp.Token = "TEST"; // For testing purposes, remove this line in production
                    return Ok(responseTemp);
                    var response = await connectionClient.ExecuteAsync<ProviderLoginResponseModel>(request);

                    _customConfig.Provider_Token = response.Data?.Token ?? "";

                    return StatusCode((int)response.StatusCode, response.Data);
                }
                else
                {
                    _logger.LogError("Error: Getting provider api url");
                    return StatusCode(StatusCodes.Status500InternalServerError, "Error: Getting provider api url");
                }
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "An error occured getting connection settings");
                return StatusCode(StatusCodes.Status500InternalServerError, "An unexpected error occurred getting connection settings");
            }
        }

        [HttpGet("ValidateProviderToken")]
        public IActionResult ValidateProviderToken()
        {
            try
            {
                var token = _customConfig.Provider_Token;
                if (String.IsNullOrWhiteSpace(token))
                {
                    return Unauthorized();
                }
                else
                {
                    return Ok(token);
                }
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "An error occured getting connection settings");
                return StatusCode(StatusCodes.Status500InternalServerError, "An unexpected error occurred getting connection settings");
            }
        }


        [HttpPost("ConsumerConnect")]
        public async Task<IActionResult> ConnectToConsumer(ConnectionSettingsModel credentials)
        {
            if (!string.IsNullOrWhiteSpace(_customConfig.Consumer_IdSrvBaseUrl) && !string.IsNullOrWhiteSpace(credentials.ClientId) && !string.IsNullOrWhiteSpace(credentials.ClientSecret))
            {
                var connectionClient = new RestClient(_customConfig.Consumer_IdSrvBaseUrl);

                var request = new RestRequest("/connect/token", Method.Post)
                    .AddParameter("grant_type", "client_credentials")
                    .AddParameter("client_id", credentials.ClientId)
                    .AddParameter("client_secret", credentials.ClientSecret)
                    .AddParameter("scope", "InvoicingAPI");

                var response = await connectionClient.ExecuteAsync(request);

                var responseObject = new ConsumerConnectionResponseModel();

                if (response.IsSuccessStatusCode && response.Content != null)
                {
                    responseObject = JsonSerializer.Deserialize<ConsumerConnectionResponseModel>(response.Content);

                    if (responseObject == null)
                    {
                        _logger.LogError("Failed to deserialize response");
                        return StatusCode(StatusCodes.Status500InternalServerError, "Failed to deserialize response");
                    }
                    else
                    {
                        return Ok(responseObject.access_token);
                    }
                }
                else
                {
                    // Response is not successful or either the content is null or empty
                    if (response.Content == null)
                    {
                        _logger.LogError("Response content is null");
                        return StatusCode((int)response.StatusCode, response);
                    }
                    else
                    {
                        if (!response.IsSuccessStatusCode)
                        {
                            _logger.LogError($"Response status is {response.Content}");
                            return StatusCode((int)response.StatusCode, response);
                        }
                        else
                        {
                            _logger.LogError($"Failed to connect to ETA: {response.Content}");
                            return StatusCode((int)response.StatusCode, response);
                        }
                    }
                }
            }
            else
            {
                // Bad Configuration **Bad Request**
                if (!string.IsNullOrWhiteSpace(_customConfig.Consumer_IdSrvBaseUrl))
                {
                    _logger.LogError("Consumer_IdSrvBaseUrl is null or empty");
                }
                if (!string.IsNullOrWhiteSpace(credentials.ClientId))
                {
                    _logger.LogError("ClientId is null or empty");
                }
                if (!string.IsNullOrWhiteSpace(credentials.ClientSecret))
                {
                    _logger.LogError("ClientSecret is null or empty");
                }
                return StatusCode(StatusCodes.Status400BadRequest, "Configuration values missing");
            }

        }

    }
}

