
namespace ETA.Integrator.Server.Dtos.ConsumerAPI
{
    public class DocumentSummaryParentDTO
    {
        public string Uuid { get; set; } = string.Empty;
        public string LongId { get; set; } = string.Empty;
        public string InternalId { get; set; } = string.Empty;
        public string TypeName { get; set; } = string.Empty;
        public string TypeVersionName { get; set; } = string.Empty;
        public string IssuerId { get; set; } = string.Empty;
        public string IssuerName { get; set; } = string.Empty;
        public string ReceiverId { get; set; } = string.Empty;
        public string ReceiverName { get; set; } = string.Empty;
        public DateTime? DateTimeIssued { get; set; }
        public decimal Total { get; set; }
        public string Status { get; set; } = string.Empty;
        public string LateSubmissionRequestNumber { get; set; } = string.Empty;
    }
}
