using Newtonsoft.Json;

namespace ETA.Integrator.Server.Models.Responses
{
    public class LoginResponseModel
    {
        [JsonProperty("UserName")]
        public string Username { get; set; } = string.Empty;
        public int UserId { get; set; }
        public string Token { get; set; } = string.Empty;
        public bool IsError { get; set; } = true;
        [JsonProperty("Msg")]
        public string Message { get; set; } = string.Empty;
    }
}
