using ETA.Integrator.Server.Helpers.Enums;

namespace ETA.Integrator.Server.Entities
{
    public class SubmittedInvoiceLog
    {
        public int Id { get; set; }
        public int InvoiceId { get; set; }
        public DateTime? SubmissionDate { get; set; }
        public InvoiceStatus Status { get; set; }
    }
}
