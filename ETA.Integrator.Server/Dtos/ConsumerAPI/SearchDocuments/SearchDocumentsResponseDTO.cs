namespace ETA.Integrator.Server.Dtos.ConsumerAPI.SearchDocuments
{
    public class SearchDocumentsResponseDTO
    {
        public List<DocumentSummaryDTO> Result { get; set; } = new();
        public SearchDocumentsMetadata Metadata { get; set; } = new();
    }

    public class SearchDocumentsMetadata
    {
        public string ContinuationToken { get; set; } = string.Empty;
    }
    
}
