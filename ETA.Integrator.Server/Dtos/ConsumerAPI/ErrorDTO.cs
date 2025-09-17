namespace ETA.Integrator.Server.Dtos.ConsumerAPI
{
    public class ErrorDTO
    {
        public string Code { get; set; } = "";  
        public string Message { get; set; } = "";
        public string? Target { get; set; }
        public List<ErrorDTO> Details { get; set; } = new();
    }
}