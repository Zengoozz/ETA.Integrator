namespace ETA.Integrator.Server.Dtos.InvoiceSubmission
{
    public class InvoiceSubmissionDTO
    {
        public string SubmissionUUID { get; init; } = string.Empty;
        public List<DocumentAcceptedDTO> AcceptedDocuments { get; init; } = new();
        public List<DocumentRejectedDTO> RejectedDocuments { get; init; } = new();
    }
}