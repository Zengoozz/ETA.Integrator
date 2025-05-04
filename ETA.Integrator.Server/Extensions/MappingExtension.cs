using ETA.Integrator.Server.Dtos;
using ETA.Integrator.Server.Models.Consumer.ETA;

namespace ETA.Integrator.Server.Extensions
{
    public static class MappingExtension
    {
        public static IssuerModel? ToIssuerModel(this IssuerDTO dto)
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
