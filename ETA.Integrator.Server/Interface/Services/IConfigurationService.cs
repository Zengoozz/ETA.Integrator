using ETA.Integrator.Server.Models;
using HMS.Core.Models.ETA;

namespace ETA.Integrator.Server.Interface.Services
{
    public interface IConfigurationService
    {
        EnvironmentModel? GetETAConfig();
        bool SetETAConfig(string clientId = "", string clientSecret = "", string generatedToken = "");
        SettingsModel GetSettings();
    }
}
