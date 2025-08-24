namespace ETA.Integrator.Server.Dtos.ConsumerAPI.SubmitDocuments
{
    public class DocumentRejectedDTO
    {
        public string InternalId { get; set; } = string.Empty;
        public Error Error { get; set; } = new();
    }
}