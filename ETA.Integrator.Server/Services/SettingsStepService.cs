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
        public async Task<IssuerDTO?> GetIssuerData()
        {
            try
            {
                var step = await _settingsStepRepository.GetByStepNumber(2);

                IssuerDTO issuerDto = !String.IsNullOrWhiteSpace(step.Data) ? JsonSerializer.Deserialize<IssuerDTO>(step.Data) ?? new IssuerDTO() : new IssuerDTO();

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
