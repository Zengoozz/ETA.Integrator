namespace ETA.Integrator.Server.Models.Consumer.ETA
{
    public class ReceiverModel
    {
        public string Type { get; set; } = string.Empty; // "B", "P", or "F"
        public string Id { get; set; } = string.Empty;
        public string Name { get; set; } = string.Empty;
        public ReceiverAddressModel Address { get; set; } = new ReceiverAddressModel();
    }
}
