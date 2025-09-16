namespace ETA.Integrator.Server.Dtos.ConsumerAPI.GetSubmission
{
    public class GetSubmissionResponseDTO
    {
        public string Uuid { get; set; } = string.Empty;
        public int DocumentCount { get; set; }
        public DateTime DateTimeReceived { get; set; }
        public string OverallStatus { get; set; } = string.Empty;
        public List<SubmissionSummaryDTO> DocumentSummary { get; set; } = new();
        public SubmissionSummaryDTO DocumentSummaryMetadata { get; set; } = new();
    }

}