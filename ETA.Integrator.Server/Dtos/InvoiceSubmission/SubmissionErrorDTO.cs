namespace ETA.Integrator.Server.Dtos.InvoiceSubmission
{

    public class SubmissionErrorDTO
    {
        public string Code { get; init; } = string.Empty;
        public string Message { get; init; } = string.Empty;
        public string Target { get; init; } = string.Empty;
        public List<SubmissionErrorDTO> Details { get; init; } = new();
    }
}