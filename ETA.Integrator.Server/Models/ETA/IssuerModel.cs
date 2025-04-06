namespace ETA.Integrator.Server.Models.ETA
{
    public class IssuerModel
    {
        public string Type { get; set; } = string.Empty; // "B", "P", or "F"
        public string Id { get; set; } = string.Empty;
        public string Name { get; set; } = string.Empty;
        public IssuerAddressModel Address { get; set; } = new IssuerAddressModel();
    }
}
