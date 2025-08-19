using ETA.Integrator.Server.Helpers.Enums;
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
        private readonly IApiCallerService _apiCallerService;

        public HttpRequestSenderService(
            IOptions<CustomConfigurations> customConfigurations,
            IApiCallerService apiCallerService
            )
        {
            _customConfig = customConfigurations.Value;
            _apiCallerService = apiCallerService;
        }

        public async Task<RestResponse> SendRequest(GenericRequest request)
        {
            RestClient client = request.ClientType switch
            {
                ClientType.Consumer => CreateConsumerClient(),
                ClientType.ConsumerAuth => CreateConsumerAuthClient(),
                ClientType.Provider => CreateProviderClient(),
                _ => throw new ProblemDetailsException(
                    statusCode: StatusCodes.Status500InternalServerError,
                    message: "UNKNOWN",
                    detail: "Unknown client type detected"
                    ),
            };

            var response = await client.ExecuteAsync<RestResponse>(request.Request);

            if (request.DoRetry && response.StatusCode == HttpStatusCode.Unauthorized) // Retry
            {
                var authToken = await AuthorizeConsumer();

                if (string.IsNullOrWhiteSpace(authToken))
                    throw new ProblemDetailsException(
                            statusCode: StatusCodes.Status500InternalServerError,
                            message: "UNKNOWN_INTERNAL_ERROR",
                            detail: "Consumer auth token has no value"
                            );

                var retryResponse = await client.ExecuteAsync<RestResponse>(request.Request);

                return retryResponse;
            }

            return response;
        }

        private async Task<string> AuthorizeConsumer()
        {
            var response = await _apiCallerService.ConnectToConsumer();
            return response.access_token;

        }
        private RestClient CreateConsumerClient()
        {
            if (_customConfig.Consumer_APIBaseUrl is null)
                throw new ProblemDetailsException(
                    StatusCodes.Status500InternalServerError,
                    "Consumer_APIBaseUrl not found",
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
