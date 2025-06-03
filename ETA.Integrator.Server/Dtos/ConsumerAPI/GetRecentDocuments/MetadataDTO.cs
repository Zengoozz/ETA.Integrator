namespace ETA.Integrator.Server.Dtos.ConsumerAPI.GetRecentDocuments
{
    public class MetadataDTO
    {
        public int TotalPages { get; set; }
        public int TotalCount { get; set; }
        public bool QueryContainsCompleteResultSet { get; set; }
        public int RemainingRecordsCount { get; set; }
    }
}
