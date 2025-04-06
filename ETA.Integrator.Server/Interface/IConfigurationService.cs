using HMS.Core.Models.ETA;

namespace ETA.Integrator.Server.Interface
{
    public interface IConfigurationService
    {
        EnvironmentModel? GetETAConfig();
        bool SetETAConfig(string clientId = "", string clientSecret = "", string generatedToken = "");
    }
}
