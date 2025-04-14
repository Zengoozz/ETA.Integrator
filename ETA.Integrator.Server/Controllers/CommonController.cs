using ETA.Integrator.Server.Interface;
using ETA.Integrator.Server.Models.Consumer.Response;
using HMS.Core.Models.ETA;
using Microsoft.AspNetCore.Mvc;
using RestSharp;
using RestSharp.Authenticators.OAuth2;

namespace ETA.Integrator.Server.Controllers
{
    [Route("HMS/[controller]")]
    [ApiController]
    public class CommonController : ControllerBase
    {
        readonly RestClient _client;
        private readonly IConfigurationService _ConfigurationService;
        public CommonController(IConfigurationService configurationService)
        {
            _ConfigurationService = configurationService;

            var config = _ConfigurationService.GetETAConfig();

            if (config != null)
            {
                EnvironmentVariableModel? baseUrl = config.Values.FirstOrDefault(x => x.Key == "apiBaseUrl") ?? null;
                EnvironmentVariableModel? token = config.Values.FirstOrDefault(x => x.Key == "generatedAccessToken") ?? null;

                if (baseUrl != null && token != null)
                {
                    var authenticator = new OAuth2AuthorizationRequestHeaderAuthenticator(token.Value, "Bearer");

                    var opt = new RestClientOptions(baseUrl.Value)
                    {
                        Authenticator = authenticator
                    };

                    _client = new RestClient(opt);
                }
                else
                {
                    throw new Exception("Error: Config attributes are missing");
                }
            }
            else
            {
                throw new Exception("Error: Config object does not exist");
            }


        }
        [HttpGet("documentTypes")]
        public async Task<IActionResult> GetDocumentTypes()
        {
            var response = await _client.GetAsync<ConsumerDocumentTypesResponseModel>("/api/v1/documenttypes");

            return Ok(response);
        }

    }
}
