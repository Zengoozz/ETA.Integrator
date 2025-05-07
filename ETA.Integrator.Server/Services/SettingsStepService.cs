using ETA.Integrator.Server.Dtos;
using ETA.Integrator.Server.Interface.Repositories;
using ETA.Integrator.Server.Interface.Services;
using ETA.Integrator.Server.Models.Core;
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

        public async Task<ConnectionDTO> GetConnectionData()
        {
            try
            {
                var step = await _settingsStepRepository.GetByStepNumber(1);

                if (step is null)
                    throw new ProblemDetailsException(
                        statusCode: StatusCodes.Status404NotFound,
                        message: "CONNECTION_NOT_FOUND",
                        detail: "Connection settings is not found."
                        );

                ConnectionDTO? connectionDto = !String.IsNullOrWhiteSpace(step.Data) ? JsonSerializer.Deserialize<ConnectionDTO>(step.Data) ?? null : null;

                if (connectionDto is null)
                    throw new ProblemDetailsException(
                        statusCode: StatusCodes.Status400BadRequest,
                        message: "CONNECTION_INVALID",
                        detail: "Connection settings is not valid."
                        );

                return connectionDto;
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Failed to get Connection Settings from database.");
                throw;
            }
        }

        public async Task<IssuerDTO> GetIssuerData()
        {
            try
            {
                var step = await _settingsStepRepository.GetByStepNumber(2);

                if (step is null)
                    throw new ProblemDetailsException(
                        statusCode: StatusCodes.Status404NotFound,
                        message: "ISSUER_NOT_FOUND",
                        detail: "Issuer settings is not found."
                        );

                IssuerDTO? issuerDto = !String.IsNullOrWhiteSpace(step.Data) ? JsonSerializer.Deserialize<IssuerDTO>(step.Data) ?? null : null;

                if (issuerDto is null)
                    throw new ProblemDetailsException(
                        statusCode: StatusCodes.Status400BadRequest,
                        message: "ISSUER_INVALID",
                        detail: "Issuer settings is not valid."
                        );

                return issuerDto;
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Failed to get issuer settings from database");
                throw;
            }
        }
    }
}
