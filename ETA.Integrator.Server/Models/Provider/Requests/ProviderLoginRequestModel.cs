namespace ETA.Integrator.Server.Models.Provider.Requests
{
    public class ProviderLoginRequestModel
    {
        public required string Email { get; set; }
        public required string Password { get; set; }
    }
}
