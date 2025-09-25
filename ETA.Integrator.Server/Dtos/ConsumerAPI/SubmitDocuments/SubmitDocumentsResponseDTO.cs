namespace ETA.Integrator.Server.Dtos.ConsumerAPI.SubmitDocuments
{
    public class SubmitDocumentsResponseDTO
    {
        public bool IsAllSuccess { get; set; } = false;
        public bool IsAllFailure { get; set; } = false;
        public string ResponseMessage { get; set; } = string.Empty;
        public bool IsError { get; set; } = false;
    }
}
