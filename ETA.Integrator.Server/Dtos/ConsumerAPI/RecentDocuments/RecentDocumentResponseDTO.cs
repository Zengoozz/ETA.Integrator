namespace ETA.Integrator.Server.Dtos.ConsumerAPI.RecentDocuments
{
    public class RecentDocumentsResponseDTO
    {
        public List<DocumentSummaryDTO> Result { get; set; } = new();
        public RecentDocumentMetadataDTO Metadata { get; set; } = new();
    }

    public class RecentDocumentMetadataDTO : MetadataParentDTO
    {
        public bool QueryContainsCompleteResultSet { get; set; }
        public int RemainingRecordsCount { get; set; }
    }
}
