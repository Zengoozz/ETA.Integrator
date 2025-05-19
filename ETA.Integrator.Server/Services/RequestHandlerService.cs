using ETA.Integrator.Server.Interface.Services;
using ETA.Integrator.Server.Models.Consumer.Response;
using ETA.Integrator.Server.Models.Core;
using Microsoft.Extensions.Options;
using RestSharp;
using RestSharp.Authenticators.OAuth2;
using System.Net;

namespace ETA.Integrator.Server.Services
{
    public class RequestHandlerService : IRequestHandlerService
    {
        private readonly CustomConfigurations _customConfig;
        private readonly ILogger<RequestHandlerService> _logger;
        private readonly ISettingsStepService _settingsStepService;

        public RequestHandlerService(
            IOptions<CustomConfigurations> customConfigurations,
            ILogger<RequestHandlerService> logger,
            ISettingsStepService settingsStepService
            )
        {
            _customConfig = customConfigurations.Value;
            _logger = logger;
            _settingsStepService = settingsStepService;
        }
        public async Task<string> AuthorizeConsumer()
        {
            #region API_CONNECT_CONFIG

            var connectionConfig = await _settingsStepService.GetConnectionData();

            // CLIENT_ID | CLIENT_SECRET VALIDATION
            if (connectionConfig == null || string.IsNullOrWhiteSpace(connectionConfig.ClientId) || string.IsNullOrWhiteSpace(connectionConfig.ClientSecret))
            {
                _logger.LogError("Failed to get the manual connection config");

                throw new ProblemDetailsException(
                    statusCode: StatusCodes.Status400BadRequest,
                    message: "CONNECTION_CONFIG_NOT_FOUND",
                    detail: "Connection configuration (Manual) not found"
                    );
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

                    return token;
                }
                else
                {
                    _logger.LogError("Failed to connect to the consumer API");
                    throw new ProblemDetailsException(
                           statusCode: StatusCodes.Status500InternalServerError,
                           message: "CONSUMER_CONNECTION_FAILED",
                           detail: "Connecting to consumer failed"
                        );
                }
            }
            else
            {
                _logger.LogError("Failed to get the consumer IdSrvURL");
                throw new ProblemDetailsException(
                           statusCode: StatusCodes.Status500InternalServerError,
                           message: "CONNECTION_URL_NOT_FOUND",
                           detail: "Connection configuration (IdSrvUrl) not found"
                        );
            }
            #endregion
        }

        public async Task<RestResponse> ExecuteWithAuthRetryAsync(RestRequest request)
        {
            var client = CreateClient();

            var response = await client.ExecuteAsync<RestResponse>(request);

            #region UNAUTHORIZED HANDLING
            if (response.StatusCode == HttpStatusCode.Unauthorized)
            {
                var authToken = await AuthorizeConsumer();

                if (!String.IsNullOrWhiteSpace(authToken))
                {
                    var retryClient = CreateClient();

                    var retryResponse = await retryClient.ExecuteAsync<RestResponse>(request);

                    return retryResponse;
                }
                else
                {
                    throw new ProblemDetailsException(
                        statusCode: StatusCodes.Status500InternalServerError,
                        message: "UNKNOWN_INTERNAL_ERROR",
                        detail: "Unknown internal error"
                        );
                }
            }
            #endregion

            return response;
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
