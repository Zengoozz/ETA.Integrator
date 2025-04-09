namespace ETA.Integrator.Server.Models.Responses
{
    public class SettingsResponseModel
    {
        public string ConnectionString { get; set; } = string.Empty;
        public string ClientId { get; set; } = string.Empty;
        public string ClientSecret { get; set; } = string.Empty;
    }
}
