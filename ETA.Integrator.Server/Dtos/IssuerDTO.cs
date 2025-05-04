using ETA.Integrator.Server.Models.Consumer.ETA;

namespace ETA.Integrator.Server.Dtos
{
    public class IssuerDTO
    {
        public string IssuerType { get; set; } = string.Empty;
        public string IssuerName { get; set; } = string.Empty;
        public string RegistrationNumber { get; set; } = string.Empty;
        public IssuerAddressModel Address { get; set; } = new IssuerAddressModel();
    }
}
