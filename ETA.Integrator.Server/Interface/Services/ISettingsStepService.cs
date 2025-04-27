using ETA.Integrator.Server.Dtos;

namespace ETA.Integrator.Server.Interface.Services
{
    public interface ISettingsStepService
    {
        Task<IssuerDTO?> GetIssuerData();
        Task<ConnectionDTO?> GetConnectionData();
    }
}
