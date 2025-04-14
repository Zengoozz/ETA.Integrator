namespace ETA.Integrator.Server.Models.Provider
{
    public class ProviderInvoiceViewModel
    {
        public string InoviceNumber { get; set; } = string.Empty;
        public string InoviceType { set; get; } = string.Empty;
        public decimal VatNet { set; get; }
        public decimal NetPrice { set; get; }
        public decimal PatShare { set; get; }
        public decimal FinShare { set; get; }
        public decimal VatFinShare { set; get; }
        public decimal VatPatShare { set; get; }
        public int InoviceId { set; get; }
        public DateTime createdDate { set; get; }
        public bool IsReviewed { set; get; } = false;
        public string InovicingNumber { set; get; } = string.Empty;
        public string FinancialClassName { set; get; } = string.Empty;
    }
}
