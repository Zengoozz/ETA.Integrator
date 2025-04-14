namespace ETA.Integrator.Server.Models.Consumer.ETA
{
    public class ValueModel
    {
        public string CurrencySold { get; set; } = string.Empty; // ISO 4217 code (e.g., "EGP")
        public decimal AmountEGP { get; set; }
        public decimal AmountSold { get; set; }
        public decimal CurrencyExchangeRate { get; set; }
    }
}
