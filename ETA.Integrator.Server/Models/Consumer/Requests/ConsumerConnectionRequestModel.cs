namespace ETA.Integrator.Server.Models.Consumer.Requests
{
    public class ConsumerConnectionRequestModel
    {
        public readonly string grant_type = "client_credentials";
        public string client_id { get; set; } = string.Empty;
        public string client_secret { get; set; } = string.Empty;
        public readonly string scope = "InvoicingAPI";
    }
}
