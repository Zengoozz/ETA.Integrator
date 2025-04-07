namespace ETA.Integrator.Server.Models.Responses
{
    public class LoginResponseModel
    {
        public string LoginToken { get; set; } = string.Empty;
        public int ExpiresIn { get; set; }
    }
}
