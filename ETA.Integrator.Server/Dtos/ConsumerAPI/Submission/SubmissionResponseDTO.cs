namespace ETA.Integrator.Server.Dtos.ConsumerAPI.Submission
{
    public class SubmissionResponseDTO
    {
        public string Uuid { get; set; } = string.Empty;
        public int DocumentCount { get; set; }
        public DateTime DateTimeReceived { get; set; }
        public string OverallStatus { get; set; } = string.Empty;
        public List<SubmissionSummaryDTO> DocumentSummary { get; set; } = new();
        public MetadataParentDTO DocumentSummaryMetadata { get; set; } = new();
    }

}