using ETA.Integrator.Server.Helpers.Enums;

namespace ETA.Integrator.Server.Entities
{
    public class InvoiceSubmissionLog
    {
        public int Id { get; set; }
        public string InternalId { get; set; } = "";
        public string SubmissionId { get; set; } = "";
        public string Uuid { get; set; } = "";
        public DateTime? SubmissionDate { get; set; }
        public InvoiceStatus Status { get; set; } = InvoiceStatus.Rejected;
        public string StatusStringfied { get; set; } = "Rejected";
        public string? RejectionReasonJSON { get; set; } = null;
    }
}
