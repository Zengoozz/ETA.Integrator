using ETA.Integrator.Server.Dtos;
using ETA.Integrator.Server.Interface.Repositories;
using ETA.Integrator.Server.Interface.Services;
using System.Text.Json;

namespace ETA.Integrator.Server.Services
{
    public class SettingsStepService : ISettingsStepService
    {
        private readonly ISettingsStepRepository _settingsStepRepository;
        private readonly ILogger<SettingsStepService> _logger;
        public SettingsStepService(ISettingsStepRepository settingsStepRepository, ILogger<SettingsStepService> logger)
        {
            _settingsStepRepository = settingsStepRepository;
            _logger = logger;
        }

        public async Task<ConnectionDTO?> GetConnectionData()
        {
            try
            {
                var step = await _settingsStepRepository.GetByStepNumber(1);

                ConnectionDTO? connectionDto = !String.IsNullOrWhiteSpace(step.Data) ? JsonSerializer.Deserialize<ConnectionDTO>(step.Data) ?? null : null;

                return connectionDto;
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Failed to get Connection Settings from DB");
                return null;
            }
        }

        public async Task<IssuerDTO?> GetIssuerData()
        {
            try
            {
                var step = await _settingsStepRepository.GetByStepNumber(2);

                IssuerDTO? issuerDto = !String.IsNullOrWhiteSpace(step.Data) ? JsonSerializer.Deserialize<IssuerDTO>(step.Data) ?? null : null;

                return issuerDto;
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Failed to get Issuer Settings from DB");
                return null;
            }
        }
    }
}
