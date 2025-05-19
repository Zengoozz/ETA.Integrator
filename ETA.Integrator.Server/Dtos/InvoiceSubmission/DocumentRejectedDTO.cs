namespace ETA.Integrator.Server.Dtos.InvoiceSubmission
{
    public class DocumentRejectedDTO
    {
        public string InternalId { get; init; } = string.Empty;
        public SubmissionErrorDTO Error { get; init; } = new();
    }
}