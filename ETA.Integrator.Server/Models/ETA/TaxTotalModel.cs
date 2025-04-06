namespace ETA.Integrator.Server.Models.ETA
{
    public class TaxTotalModel
    {
        public string TaxType { get; set; } = string.Empty;// Tax type code
        public decimal Amount { get; set; }
    }
}
