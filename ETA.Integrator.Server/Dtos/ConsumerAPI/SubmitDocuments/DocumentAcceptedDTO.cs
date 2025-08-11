namespace ETA.Integrator.Server.Dtos.ConsumerAPI.SubmitDocuments
{
    public class DocumentAcceptedDTO
    {
        public string Uuid { get; set; } = string.Empty;
        public string LongId { get; set; } = string.Empty;
        public string InternalId { get; set; } = string.Empty;
    }
}