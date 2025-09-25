namespace ETA.Integrator.Server.Dtos
{
    public class ProviderInvoicesSearchDTO
    {
        public DateTime? StartDate { set; get; }
        public DateTime? EndDate { set; get; }
        public string InvoiceType { set; get; } = "I";// Claim | Invoice
        public List<string> InvoicesIds { set; get; } = new();
    }
}
