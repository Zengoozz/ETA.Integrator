using ETA.Integrator.Server.Interface.Services;
using ETA.Integrator.Server.Interface.Services.Common;
using ETA.Integrator.Server.Models.Consumer.Response;
using ETA.Integrator.Server.Models.Core;
using Microsoft.Extensions.Options;
using RestSharp;
using RestSharp.Authenticators.OAuth2;
using System.Net;

namespace ETA.Integrator.Server.Services.Common
{
    public class HttpRequestSenderService : IHttpRequestSenderService
    {
        private readonly CustomConfigurations _customConfig;
        private readonly ILogger<HttpRequestSenderService> _logger;
        private readonly ISettingsStepService _settingsStepService;

        public HttpRequestSenderService(
            IOptions<CustomConfigurations> customConfigurations,
            ILogger<HttpRequestSenderService> logger,
            ISettingsStepService settingsStepService
            )
        {
            _customConfig = customConfigurations.Value;
            _logger = logger;
            _settingsStepService = settingsStepService;
        }

        public async Task<RestResponse> ExecuteWithAuthRetryAsync(RestRequest request)
        {
            var client = CreateConsumerClient();
            var response = await client.ExecuteAsync<RestResponse>(request);

            if (response.StatusCode == HttpStatusCode.Unauthorized) // Retry
            {
                var authToken = await AuthorizeConsumer();

                if (string.IsNullOrWhiteSpace(authToken))
                    throw new ProblemDetailsException(
                            statusCode: StatusCodes.Status500InternalServerError,
                            message: "HttpRequestSenderConsumerService/ExecuteWithAuthRetryAsync: UNKNOWN_INTERNAL_ERROR",
                            detail: "Consumer auth token has no value"
                            );

                var retryResponse = await client.ExecuteAsync<RestResponse>(request);

                return retryResponse;
            }

            return response;
        }

        public async Task<RestResponse> SendConsumerAuthRequest(RestRequest request)
        {
            var client = CreateConsumerAuthClient();
            var response = await client.ExecuteAsync(request);

            return response;
        }

        private async Task<string> AuthorizeConsumer()
        {
            var connectionConfig = await _settingsStepService.GetConnectionData();
            var token = "";

            // CLIENT_ID | CLIENT_SECRET VALIDATION
            if (connectionConfig == null || string.IsNullOrWhiteSpace(connectionConfig.ClientId) || string.IsNullOrWhiteSpace(connectionConfig.ClientSecret))
            {
                _logger.LogError("Failed to get the manual connection config");

                throw new ProblemDetailsException(
                    statusCode: StatusCodes.Status400BadRequest,
                    message: "NOT_FOUND",
                    detail: "Connection configuration (Manual) not found"
                    );
            }
            // SERV API URL VALIDATION
            if (string.IsNullOrWhiteSpace(_customConfig.Consumer_IdSrvBaseUrl))
            {
                _logger.LogError("Failed to get the consumer IdSrvURL");
                throw new ProblemDetailsException(
                           statusCode: StatusCodes.Status400BadRequest,
                           message: "NOT_FOUND",
                           detail: "Connection configuration (IdSrvUrl) not found"
                        );
            }

            // GENERATING CONSUMER TOKEN
            var authOpt = new RestClientOptions(_customConfig.Consumer_IdSrvBaseUrl);
            var authClient = new RestClient(authOpt);

            var authRequest = new RestRequest("/connect/token", Method.Post)
                .AddParameter("grant_type", "client_credentials")
                .AddParameter("client_id", connectionConfig.ClientId)
                .AddParameter("client_secret", connectionConfig.ClientSecret)
                .AddParameter("scope", "InvoicingAPI");

            var response = await authClient.ExecuteAsync<ConsumerConnectionResponseModel>(authRequest);

            if (response.Data is null || !response.IsSuccessStatusCode)
            {
                _logger.LogError("Failed to connect to the consumer API");
                throw new ProblemDetailsException(
                       statusCode: StatusCodes.Status401Unauthorized,
                       message: "UNAUTHORIZED",
                       detail: "failed to connect to consumer (not authorized)"
                    );
            }

            token = response.Data.access_token;
            _customConfig.Consumer_Token = token;

            return token;
        }

        private RestClient CreateConsumerClient()
        {
            if (_customConfig.Consumer_APIBaseUrl is null)
                throw new ProblemDetailsException(
                    StatusCodes.Status500InternalServerError,
                    "HttpRequestSenderService/CreateConsumerClient: Consumer_APIBaseUrl not found",
                    "Consumer api base url not found"
                    );

            var authenticator = new OAuth2AuthorizationRequestHeaderAuthenticator(_customConfig.Consumer_Token, "Bearer");
            var submitOpt = new RestClientOptions(_customConfig.Consumer_APIBaseUrl)
            {
                Authenticator = authenticator
            };

            var client = new RestClient(submitOpt);

            return client;
        }
        private RestClient CreateConsumerAuthClient()
        {
            if (string.IsNullOrWhiteSpace(_customConfig.Consumer_IdSrvBaseUrl))
                throw new ProblemDetailsException(
                           statusCode: StatusCodes.Status400BadRequest,
                           message: "NOT_FOUND",
                           detail: "Consumer service api base url not found"
                        );

            var opt = new RestClientOptions(_customConfig.Consumer_IdSrvBaseUrl);
            var client = new RestClient(opt);

            return client;
        }
        private RestClient CreateProviderClient()
        {
            if (_customConfig.Provider_APIURL is null)
                throw new ProblemDetailsException(
                    StatusCodes.Status500InternalServerError,
                    "Provider_APIURL not found",
                    "Getting provider api url failed"
                    );

            var opt = new RestClientOptions(_customConfig.Provider_APIURL);

            var client = new RestClient(opt);

            return client;
        }

    }
}
