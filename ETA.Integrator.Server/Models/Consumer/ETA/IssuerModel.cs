using ETA.Integrator.Server.Dtos;

namespace ETA.Integrator.Server.Models.Consumer.ETA
{
    public class IssuerModel
    {
        public string Type { get; set; } = string.Empty; // "B", "P", or "F"
        public string Id { get; set; } = string.Empty;
        public string Name { get; set; } = string.Empty;
        public IssuerAddressModel Address { get; set; } = new IssuerAddressModel();
    }
    public static class IssuerMapper
    {
        public static IssuerModel? FromDTO(this IssuerDTO dto)
        {
            if (dto == null)
                return null;

            return new IssuerModel
            {
                Type = dto.IssuerType,
                Id = dto.RegistrationNumber,
                Name = dto.IssuerName,
                Address = dto.Address
            };
        }
    }
}
