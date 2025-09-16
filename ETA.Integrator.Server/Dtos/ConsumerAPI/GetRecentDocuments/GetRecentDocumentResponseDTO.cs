namespace ETA.Integrator.Server.Dtos.ConsumerAPI.GetRecentDocuments
{
    public class GetRecentDocumentsResponseDTO
    {
        public List<DocumentSummaryDTO> Result { get; set; } = new();
        public GetRecentDocumentMetadataDTO Metadata { get; set; } = new();
    }

    public class GetRecentDocumentMetadataDTO : MetadataParentDTO
    {
        public bool QueryContainsCompleteResultSet { get; set; }
        public int RemainingRecordsCount { get; set; }
    }
}
