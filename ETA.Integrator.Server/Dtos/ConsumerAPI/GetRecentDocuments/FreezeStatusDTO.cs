namespace ETA.Integrator.Server.Dtos.ConsumerAPI.GetRecentDocuments
{
    public class FreezeStatusDTO
    {
        public bool Frozen { get; set; }
        public int Type { get; set; }
        public int Scope { get; set; }
        public DateTime ActionDate { get; set; }
        public string AuCode { get; set; } = string.Empty;
        public string AuName { get; set; } = string.Empty;
    }
}
