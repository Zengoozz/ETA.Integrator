namespace ETA.Integrator.Server.Dtos.ConsumerAPI.SubmitDocuments
{
    public class Error
    {
        public string Code { get; set; } = "";  
        public string Message { get; set; } = "";
        public string? Target { get; set; }
        public List<Error> Details { get; set; } = new();
    }
}