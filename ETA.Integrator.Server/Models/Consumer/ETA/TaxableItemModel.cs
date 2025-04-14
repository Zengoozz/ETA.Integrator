namespace ETA.Integrator.Server.Models.Consumer.ETA
{
    public class TaxableItemModel
    {
        public string TaxType { get; set; } = string.Empty; // Tax type code
        public decimal Amount { get; set; }
        public string SubType { get; set; } = string.Empty;
        public decimal Rate { get; set; } // Tax rate (0 to 999)
    }
}
