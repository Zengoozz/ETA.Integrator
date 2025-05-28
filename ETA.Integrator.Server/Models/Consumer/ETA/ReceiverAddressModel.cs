namespace ETA.Integrator.Server.Models.Consumer.ETA
{
    public class ReceiverAddressModel
    {
        public string Country { get; set; } = string.Empty; // ISO-3166-2 code (e.g., "EG")
        public string Governate { get; set; } = string.Empty;
        public string RegionCity { get; set; } = string.Empty;
        public string Street { get; set; } = string.Empty;
        public string BuildingNumber { get; set; } = string.Empty;
        public string PostalCode { get; set; } = string.Empty;
        public string Floor { get; set; } = string.Empty;
        public string Room { get; set; } = string.Empty;
        public string Landmark { get; set; } = string.Empty;
        public string AdditionalInformation { get; set; } = string.Empty;
    }
    //public static class ReceiverAddressMapper
    //{
    //    public static ReceiverAddressModel FromString(this string address)
    //    {
    //        if (string.IsNullOrWhiteSpace(address))
    //            return new ReceiverAddressModel();

    //        var addressParts = address.Split('|');
    //        return new ReceiverAddressModel
    //        {
    //            BuildingNumber = addressParts[0],
    //            Street = addressParts[1],
    //            RegionCity = addressParts[2],
    //            Governate = addressParts[3],
    //            Country = addressParts[4].Trim()
    //        };
    //    }
    //}
}
