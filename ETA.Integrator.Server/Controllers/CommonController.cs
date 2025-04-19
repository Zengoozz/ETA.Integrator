using ETA.Integrator.Server.Interface.Repositories;
using ETA.Integrator.Server.Models.Consumer.Response;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Extensions.Options;
using RestSharp;
using RestSharp.Authenticators.OAuth2;

namespace ETA.Integrator.Server.Controllers
{
    [Route("HMS/[controller]")]
    [ApiController]
    public class CommonController : ControllerBase
    {
        readonly RestClient _client;
        private readonly CustomConfigurations _customConfig;

        private readonly ILogger<ConfigController> _logger;

        private readonly ISettingsStepRepository _settingsStepRepository;

        public CommonController(
                        IOptions<CustomConfigurations> customConfigurations,
            ILogger<ConfigController> logger,
                        ISettingsStepRepository settingsStepRepository
            )
        {

            _customConfig = customConfigurations.Value;
            _logger = logger;
            _settingsStepRepository = settingsStepRepository;

            var token = "";

            var connectionSettings = _settingsStepRepository.GetByStepNumber(1);

            if (_customConfig.Consumer_APIBaseUrl != null && token != null)
            {
                var authenticator = new OAuth2AuthorizationRequestHeaderAuthenticator(token, "Bearer");

                var opt = new RestClientOptions(_customConfig.Consumer_APIBaseUrl)
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
        [HttpGet("documentTypes")]
        public async Task<IActionResult> GetDocumentTypes()
        {
            try
            {
                var response = await _client.GetAsync<ConsumerDocumentTypesResponseModel>("/api/v1/documenttypes");

                return Ok(response);
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "An error occured while retrieving document types");
                return StatusCode(StatusCodes.Status500InternalServerError, "An unexpected error occured while retrieving document types");
            }
        }

    }
}
