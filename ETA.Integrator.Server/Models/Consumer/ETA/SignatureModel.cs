namespace ETA.Integrator.Server.Models.Consumer.ETA
{
    public class SignatureModel
    {
        public string SignatureType { get; set; } = "I"; // "I" (Issuer) or "S" (Service Provider)
        public string Value { get; set; } = string.Empty;// Base64-encoded CADES-BES structure
    }
}
