using ETA.Integrator.Server.Helpers.Enums;

namespace ETA.Integrator.Server.Dtos
{
    public class UpdateSubmissionStatusDTO
    {
        public int LoggedId { get; set; }
        public DateTime? SubmissionTime { get; set; }
        public InvoiceStatus SubmissionsStatus { get; set; }
    }
}
