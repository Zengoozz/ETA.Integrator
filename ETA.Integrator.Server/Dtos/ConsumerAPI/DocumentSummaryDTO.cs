using ETA.Integrator.Server.Dtos.ConsumerAPI.Submission;

namespace ETA.Integrator.Server.Dtos.ConsumerAPI
{
    public class DocumentSummaryDTO : SubmissionSummaryDTO
    {
        public string SubmissionUUID { get; set; } = string.Empty;
        public string PublicUrl { get; set; } = string.Empty;
        public string IssuerType { get; set; } = string.Empty;
        public string ReceiverType { get; set; } = string.Empty;
        public DateTime? DateTimeReceived { get; set; }
        public decimal TotalSales { get; set; }
        public decimal TotalDiscounts { get; set; }
        public decimal NetAmount { get; set; }
        public DateTime? CancelRequestDate { get; set; }
        public DateTime? RejectRequestDate { get; set; }
        public DateTime? CancelRequestDelayedDate { get; set; }
        public DateTime? RejectRequestDelayedDate { get; set; }
        public DateTime? DeclineCancelRequestDate { get; set; }
        public DateTime? DeclineRejectRequestDate { get; set; }
        public string DocumentStatusReason { get; set; } = string.Empty;
        public string CreatedByUserId { get; set; } = string.Empty;
        public FreezeStatusDTO FreezeStatus { get; set; } = new();
    }

    public class FreezeStatusDTO
    {
        public bool Frozen { get; set; }
        public int? Type { get; set; }
        public int? Scope { get; set; }
        public DateTime? ActionDate { get; set; }
        public string AuCode { get; set; } = string.Empty;
        public string AuName { get; set; } = string.Empty;
    }
}
