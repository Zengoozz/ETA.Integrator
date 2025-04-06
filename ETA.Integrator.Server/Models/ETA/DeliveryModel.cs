namespace ETA.Integrator.Server.Models.ETA
{
    public class DeliveryModel
    {
        public string Approach { get; set; } = string.Empty;
        public string Packaging { get; set; } = string.Empty;
        public string DateValidity { get; set; } = string.Empty;
        public string ExportPort { get; set; } = string.Empty;
        public string CountryOfOrigin { get; set; } = string.Empty; // ISO-3166-2 code (e.g., "SA")
        public decimal GrossWeight { get; set; }
        public decimal NetWeight { get; set; }
        public string Terms { get; set; } = string.Empty;
    }
}
