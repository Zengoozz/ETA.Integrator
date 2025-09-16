namespace ETA.Integrator.Server.Dtos.ConsumerAPI.SubmitDocuments
{
    public class SuccessfulResponseDTO
    {
        public string SubmissionId { get; set; } = string.Empty;
        public List<DocumentAcceptedDTO> AcceptedDocuments { get; set; } = new();
        public List<DocumentRejectedDTO> RejectedDocuments { get; set; } = new();
    }
}
