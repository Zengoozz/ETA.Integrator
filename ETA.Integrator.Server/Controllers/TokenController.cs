using Microsoft.AspNetCore.Mvc;

namespace ETA.Integrator.Server.Controllers
{
    [ApiController]
    [Route("HMS/[controller]")]
    public class TokenController : ControllerBase
    {
        public TokenController()
        {
        }
        [HttpGet("Test")]
        public IActionResult Test()
        {
            return Ok();
        }

        //[HttpGet("Connect")]
        //public async Task<IActionResult?> ConnectToETA()
        //{
        //    var config = _ConfigurationService.GetETAConfig();

        //    if (config != null)
        //    {
        //        EnvironmentVariableModel? idSrvUrl = config.Values.FirstOrDefault(x => x.Key == "idSrvBaseUrl") ?? null;
        //        EnvironmentVariableModel? clientId = config.Values.FirstOrDefault(x => x.Key == "clientId") ?? null;
        //        EnvironmentVariableModel? clientSecret = config.Values.FirstOrDefault(x => x.Key == "clientSecret") ?? null;

        //        if (idSrvUrl != null && clientId != null && clientSecret != null)
        //        {
        //            var connectionClient = new RestClient(idSrvUrl.Value);

        //            var request = new RestRequest("/connect/token", Method.Post)
        //                .AddParameter("grant_type", "client_credentials")
        //                .AddParameter("client_id", clientId.Value)
        //                .AddParameter("client_secret", clientSecret.Value)
        //                .AddParameter("scope", "InvoicingAPI");

        //            var response = await connectionClient.ExecuteAsync(request);

        //            var responseObject = new ConsumerConnectionResponseModel();

        //            if (response.Content != null)
        //            {
        //                responseObject = JsonSerializer.Deserialize<ConsumerConnectionResponseModel>(response.Content);
        //            }

        //            if (responseObject != null)
        //            {
        //                _ConfigurationService.SetETAConfig(generatedToken: responseObject.access_token);
        //                return Ok(responseObject);
        //            }

        //        }

        //        return StatusCode(StatusCodes.Status500InternalServerError, "Configuration values missing");
        //    }
        //    else
        //    {
        //        return StatusCode(StatusCodes.Status500InternalServerError, "No configuration file");
        //    }
        //}
    }
}
