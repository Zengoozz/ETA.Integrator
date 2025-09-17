using ETA.Integrator.Server.Dtos;
using ETA.Integrator.Server.Interface.Services.Common;
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
        private readonly IApiCallerService _apiCallerService;

        public AuthController(
            IOptions<CustomConfigurations> customConfig,
            ILogger<AuthController> logger,
            IApiCallerService apiCallerService
            )
        {
            _customConfig = customConfig.Value;
            _logger = logger;
            _apiCallerService = apiCallerService;
        }

        [HttpPost("ProviderConnect")]
        public async Task<IActionResult> ConnectToProvider([FromBody] ProviderLoginRequestModel model)
        {
            //var responseTemp = new ProviderLoginResponseModel();
            //responseTemp.Token = "TEST"; // For testing purposes, remove this line in production
            //return Ok(responseTemp);

            var response = await _apiCallerService.ConnectToProvider(model);

            return Ok(response);
        }

        [HttpGet("ValidateProviderToken")]
        public IActionResult ValidateProviderToken()
        {
            var token = _customConfig.Provider_Token;

            if (String.IsNullOrWhiteSpace(token))
                return Unauthorized("Provider token is missing. Try to login again.");
            else
                return Ok(token);
        }


        [HttpPost("ConsumerConnect")]
        public async Task<IActionResult> ConnectToConsumer(ConnectionDTO? credentials = null)
        {
            var response = await _apiCallerService.ConnectToConsumer(credentials);

            return Ok(response);
        }

    }
}

