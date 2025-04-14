namespace ETA.Integrator.Server.Models.Consumer.Response
{
    public class ConsumerConnectionResponseModel
    {
        public string access_token { get; set; } = string.Empty;
        public int expires_in { get; set; }
        public string token_type { get; set; } = string.Empty;
        public string scope { get; set; } = string.Empty;
    }
}
