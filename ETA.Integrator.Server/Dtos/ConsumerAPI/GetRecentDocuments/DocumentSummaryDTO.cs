namespace ETA.Integrator.Server.Dtos.ConsumerAPI.GetRecentDocuments
{
    public class DocumentSummaryDTO
    {
        public string Uuid { get; set; } = string.Empty;
        public string SubmissionUUID { get; set; } = string.Empty;
        public string LongId { get; set; } = string.Empty;
        public string PublicUrl { get; set; } = string.Empty;
        public string InternalId { get; set; } = string.Empty;
        public string TypeName { get; set; } = string.Empty;
        public string TypeVersionName { get; set; } = string.Empty;
        public string IssuerId { get; set; } = string.Empty;
        public string IssuerName { get; set; } = string.Empty;
        public string IssuerType { get; set; } = string.Empty;
        public string ReceiverId { get; set; } = string.Empty;
        public string ReceiverName { get; set; } = string.Empty;
        public string ReceiverType { get; set; } = string.Empty;
        public DateTime? DateTimeIssued { get; set; }
        public DateTime? DateTimeReceived { get; set; }
        public decimal TotalSales { get; set; }
        public decimal TotalDiscounts { get; set; }
        public decimal NetAmount { get; set; }
        public decimal Total { get; set; }
        public string Status { get; set; } = string.Empty;
        public DateTime? CancelRequestDate { get; set; }
        public DateTime? RejectRequestDate { get; set; }
        public DateTime? CancelRequestDelayedDate { get; set; }
        public DateTime? RejectRequestDelayedDate { get; set; }
        public DateTime? DeclineCancelRequestDate { get; set; }
        public DateTime? DeclineRejectRequestDate { get; set; }
        public string DocumentStatusReason { get; set; } = string.Empty;
        public string CreatedByUserId { get; set; } = string.Empty;
        public FreezeStatusDTO FreezeStatus { get; set; } = new();
        public string LateSubmissionRequestNumber { get; set; } = string.Empty;
    }
}
