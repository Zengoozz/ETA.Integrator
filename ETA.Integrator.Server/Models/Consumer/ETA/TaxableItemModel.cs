namespace ETA.Integrator.Server.Models.Consumer.ETA
{
    public class TaxableItemModel
    {
        public string TaxType { get; set; } = "T1"; // Tax type code
        public decimal Amount { get; set; } = 0;
        public string SubType { get; set; } = "V001";
        public decimal Rate { get; set; } = 0; // Tax rate (0 to 999)
    }
}
