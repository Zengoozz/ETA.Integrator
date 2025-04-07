using ETA.Integrator.Server.Models.Requests;
using Microsoft.AspNetCore.Mvc;
using RestSharp;

namespace ETA.Integrator.Server.Controllers
{
    [Route("HMS/[controller]")]
    [ApiController]
    public class LoginController : ControllerBase
    {
        readonly RestClient _client;
        public LoginController()
        {
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
                    .AddParameter("Email", model.Username)
                    .AddParameter("Password", model.Password);

                var res = await _client.ExecuteAsync(request);

                return Ok("Hello");
            }
            catch (Exception)
            {
                throw;
            }
        }
    }
}
