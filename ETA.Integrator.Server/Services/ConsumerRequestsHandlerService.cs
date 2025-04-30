using ETA.Integrator.Server.Interface.Services;
using ETA.Integrator.Server.Models.Consumer.Response;
using Microsoft.Extensions.Options;
using RestSharp;
using RestSharp.Authenticators.OAuth2;
using System.Net;

namespace ETA.Integrator.Server.Services
{
    public class ConsumerRequestsHandlerService : IConsumerRequestsHandlerService
    {
        private readonly CustomConfigurations _customConfig;
        private readonly ILogger<ConsumerRequestsHandlerService> _logger;
        private readonly ISettingsStepService _settingsStepService;

        public ConsumerRequestsHandlerService(
            IOptions<CustomConfigurations> customConfigurations,
            ILogger<ConsumerRequestsHandlerService> logger,
            ISettingsStepService settingsStepService
            )
        {
            _customConfig = customConfigurations.Value;
            _logger = logger;
            _settingsStepService = settingsStepService;
        }
        public async Task<GenericResponse<string>> AuthorizeConsumer()
        {
            #region API_CONNECT_CONFIG

            var connectionConfig = await _settingsStepService.GetConnectionData();

            // CLIENT_ID | CLIENT_SECRET VALIDATION
            if (connectionConfig == null || string.IsNullOrWhiteSpace(connectionConfig.ClientId) || string.IsNullOrWhiteSpace(connectionConfig.ClientSecret))
            {
                _logger.LogError("Failed to get the manual connection config");

                return new GenericResponse<string>
                {
                    StatusCode = (int)HttpStatusCode.BadRequest,
                    Success = false,
                    Code = "CONNECTION_CONFIG_NOT_FOUND",
                    Message = "Connection configuration (Manual) not found",
                    Data = null
                }
                ;
            }

            var token = "";

            // GENERATING CONSUMER TOKEN
            if (!String.IsNullOrWhiteSpace(_customConfig.Consumer_IdSrvBaseUrl))
            {
                var authOpt = new RestClientOptions(_customConfig.Consumer_IdSrvBaseUrl);
                var authClient = new RestClient(authOpt);

                var authRequest = new RestRequest("/connect/token", Method.Post)
                    .AddParameter("grant_type", "client_credentials")
                    .AddParameter("client_id", connectionConfig.ClientId)
                    .AddParameter("client_secret", connectionConfig.ClientSecret)
                    .AddParameter("scope", "InvoicingAPI");

                var response = await authClient.ExecuteAsync<ConsumerConnectionResponseModel>(authRequest);

                if (response.Data != null && response.IsSuccessStatusCode)
                {
                    token = response.Data.access_token;

                    _customConfig.Consumer_Token = token;

                    return new GenericResponse<string>
                    {
                        StatusCode = (int)HttpStatusCode.OK,
                        Success = true,
                        Code = "TOKEN_CREATED",
                        Message = "token created",
                        Data = token
                    };
                }
                else
                {
                    _logger.LogError("Failed to connect to the consumer API");
                    return new GenericResponse<string>
                    {
                        StatusCode = (int)HttpStatusCode.InternalServerError,
                        Success = false,
                        Code = "CONSUMER_CONNECTION_FAILED",
                        Message = "Connecting to consumer failed",
                        Data = null
                    };
                }
            }
            else
            {
                _logger.LogError("Failed to get the consumer IdSrvURL");
                return new GenericResponse<string>
                {
                    StatusCode = (int)HttpStatusCode.InternalServerError,
                    Success = false,
                    Code = "CONNECTION_URL_NOT_FOUND",
                    Message = "Connection configuration (IdSrvUrl) not found",
                    Data = null
                };
            }
            #endregion
        }

        public async Task<GenericResponse<RestResponse?>> ExecuteWithAuthRetryAsync(RestRequest request)
        {
            var client = CreateClient();

            var response = await client.ExecuteAsync<RestResponse>(request);

            #region UNAUTHORIZED HANDLING
            if (response.StatusCode == HttpStatusCode.Unauthorized)
            {
                var authResponse = await AuthorizeConsumer();

                if (authResponse.Success)
                {
                    var retryClient = CreateClient();

                    var retryResponse = await retryClient.ExecuteAsync<RestResponse>(request);

                    return new GenericResponse<RestResponse?>
                    {
                        StatusCode = (int)retryResponse.StatusCode,
                        Success = retryResponse.IsSuccessful,
                        Code = retryResponse.ErrorMessage?.ToUpper() ?? "",
                        Message = retryResponse.ErrorMessage ?? "",
                        Data = retryResponse
                    };
                }
                else if (authResponse != null && !authResponse.Success)
                {
                    return new GenericResponse<RestResponse?>
                    {
                        StatusCode = authResponse.StatusCode,
                        Success = authResponse.Success,
                        Code = authResponse.Code,
                        Message = authResponse.Message,
                        Data = null
                    };
                }
                else
                {
                    _logger.LogError("Unknown internal error");
                    return new GenericResponse<RestResponse?>
                    {
                        StatusCode = StatusCodes.Status500InternalServerError,
                        Success = false,
                        Code = "UNKNOWN_INTERNAL_ERROR",
                        Message = "Unknown internal error",
                        Data = null
                    };
                }
            }
            #endregion

            return new GenericResponse<RestResponse?>
            {
                StatusCode = (int)response.StatusCode,
                Success = response.IsSuccessful,
                Code = response.ErrorMessage?.ToUpper() ?? "",
                Message = response.ErrorMessage ?? "",
                Data = response
            };
        }

        private RestClient CreateClient()
        {
            var authenticator = new OAuth2AuthorizationRequestHeaderAuthenticator(_customConfig.Consumer_Token, "Bearer");

            var submitOpt = new RestClientOptions(_customConfig.Consumer_APIBaseUrl)
            {
                Authenticator = authenticator
            };

            var client = new RestClient(submitOpt);

            return client;
        }

    }
}
