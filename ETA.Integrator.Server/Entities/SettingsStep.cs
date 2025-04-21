namespace ETA.Integrator.Server.Entities
{
    public class SettingsStep
    {
        public int Order { get; set; }
        public string Name { get; set; } = string.Empty;
        public string? Data { get; set; } = null;
    }
}
