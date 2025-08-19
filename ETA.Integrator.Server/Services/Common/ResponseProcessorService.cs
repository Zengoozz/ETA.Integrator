using ETA.Integrator.Server.Dtos.ConsumerAPI.GetRecentDocuments;
using ETA.Integrator.Server.Dtos.ConsumerAPI.SubmitDocuments;
using ETA.Integrator.Server.Interface.Services.Common;
using ETA.Integrator.Server.Models.Consumer.Response;
using ETA.Integrator.Server.Models.Core;
using ETA.Integrator.Server.Models.Provider;
using ETA.Integrator.Server.Models.Provider.Response;
using RestSharp;
using System.Text.Json;

namespace ETA.Integrator.Server.Services.Common
{
    public class ResponseProcessorService : IResponseProcessorService
    {
        public Task<ProviderLoginResponseModel> ConnectToProvider(RestResponse response)
        {
            ProviderLoginResponseModel? serializedResponse = new();

            if (response is not null && response.Content is not null && response.IsSuccessful)
            {
                try
                {
                    var options = new JsonSerializerOptions
                    {
                        PropertyNamingPolicy = JsonNamingPolicy.CamelCase,
                        PropertyNameCaseInsensitive = true
                    };

                    serializedResponse = JsonSerializer.Deserialize<ProviderLoginResponseModel>(response.Content, options);
                }
                catch (Exception ex)
                {
                    Console.WriteLine("JSON Error: " + ex.Message);
                    throw;
                }

                if (serializedResponse is null)
                    throw new ProblemDetailsException(
                        statusCode: StatusCodes.Status500InternalServerError,
                        message: "SERIALIZATION_FAILED",
                        detail: "Could not serialize the response."
                        );

                if (serializedResponse.IsError)
                    throw new ProblemDetailsException(
                        statusCode: StatusCodes.Status401Unauthorized,
                        message: "UNAUTHORIZED",
                        detail: String.IsNullOrEmpty(serializedResponse.Message) ? "No further details" : serializedResponse.Message
                        );
            }
            else
            {
                throw new ProblemDetailsException(
                        statusCode: StatusCodes.Status500InternalServerError,
                        message: "LOGIN_FAILED",
                        detail: response?.ErrorMessage ?? serializedResponse.Message
                        );
            }

            return Task.FromResult(serializedResponse);
        }
        public Task<ConsumerConnectionResponseModel> ConnectToConsumer(RestResponse response)
        {
            ConsumerConnectionResponseModel? serializedResponse = new();

            if (response is null || response.Content is null || !response.IsSuccessful)
            {
                var detailMsg = response is null || response.Content is null ? "No response to process" : "API response failure";

                throw new ProblemDetailsException(
                        statusCode: StatusCodes.Status500InternalServerError,
                        message: "RESPONSE_ERR",
                        detail: detailMsg
                        );
            }

            try
            {
                var options = new JsonSerializerOptions
                {
                    PropertyNamingPolicy = JsonNamingPolicy.CamelCase,
                    PropertyNameCaseInsensitive = true
                };

                serializedResponse = JsonSerializer.Deserialize<ConsumerConnectionResponseModel>(response.Content, options);
            }
            catch (JsonException)
            {
                throw;
            }

            if (serializedResponse is null)
                throw new ProblemDetailsException(
                    statusCode: StatusCodes.Status500InternalServerError,
                    message: "SERIALIZATION_FAILED",
                    detail: "Could not serialize the response."
                    );

            return Task.FromResult(serializedResponse);

        }
        public Task<List<ProviderInvoiceViewModel>> GetProviderInvoices(RestResponse response)
        {
            List<ProviderInvoiceViewModel>? serializedResponse = new();
            if (response != null && (int)response.StatusCode == StatusCodes.Status200OK && response.Content != null)
            {
                try
                {
                    var options = new JsonSerializerOptions
                    {
                        PropertyNamingPolicy = JsonNamingPolicy.CamelCase,
                        PropertyNameCaseInsensitive = true
                    };

                    serializedResponse = JsonSerializer.Deserialize<List<ProviderInvoiceViewModel>>(response.Content, options);
                }
                catch (JsonException)
                {
                    throw;
                }
            }
            else
            {
                throw new ProblemDetailsException(
                   (int?)response?.StatusCode ?? StatusCodes.Status500InternalServerError,
                   "PROVIDER_SERVER_ERR",
                    response?.Content ?? "response content is null"
                   );
            }

            if (serializedResponse is null)
                throw new ProblemDetailsException(
                    statusCode: StatusCodes.Status500InternalServerError,
                    message: "SERIALIZATION_FAILED",
                    detail: "Could not serialize the response."
                    );


            return Task.FromResult(serializedResponse);
        }
        public Task<GetRecentDocumentsResponseDTO> GetRecentDocuments(RestResponse response)
        {
            GetRecentDocumentsResponseDTO? serializedResponse = new();

            if (response is null || !response.IsSuccessful || (int)response.StatusCode != StatusCodes.Status200OK || response.Content is null)
            {
                if (response is null)
                    throw new ProblemDetailsException(
                        statusCode: StatusCodes.Status500InternalServerError,
                        message: "BAD_PARAMS",
                        detail: "No response to process."
                        );

                else if ((int)response.StatusCode == StatusCodes.Status403Forbidden)
                    throw new ProblemDetailsException(
                        statusCode: StatusCodes.Status403Forbidden,
                        message: "CONSUMER_FAILURE",
                        detail: response.ErrorMessage ?? response.Content ?? "Unexpected error"
                        );

                else
                    throw new ProblemDetailsException(
                        statusCode: StatusCodes.Status500InternalServerError,
                        message: "UNKNOWN",
                        detail: response.ErrorMessage ?? response.Content ?? "Unexpected error"
                        );
            }

            if ((int)response.StatusCode == StatusCodes.Status200OK)
            {
                try
                {
                    var options = new JsonSerializerOptions
                    {
                        PropertyNamingPolicy = JsonNamingPolicy.CamelCase,
                        PropertyNameCaseInsensitive = true
                    };

                    serializedResponse = JsonSerializer.Deserialize<GetRecentDocumentsResponseDTO>(response.Content, options);
                }
                catch (JsonException)
                {
                    throw;
                }
            }

            if (serializedResponse is null)
                throw new ProblemDetailsException(
                    statusCode: StatusCodes.Status500InternalServerError,
                    message: "SERIALIZATION_FAILED",
                    detail: "Could not serialize the response."
                    );

            return Task.FromResult(serializedResponse);
        }
        public Task<SubmitDocumentsResponseDTO> SubmitDocuments(RestResponse response)
        {
            if (response is null)
                throw new ProblemDetailsException(
                    statusCode: StatusCodes.Status500InternalServerError,
                    message: "BAD_PARAMS",
                    detail: "No response to process."
                    );

            if ((int)response.StatusCode == StatusCodes.Status422UnprocessableEntity)
            {
                var errDetails = "Unexpected error";

                if (response.Headers is not null && response.Headers.Count > 0)
                {
                    var retryAfterHeader = response.Headers
                        .FirstOrDefault(h => h.Name.Equals("Retry-After", StringComparison.OrdinalIgnoreCase));

                    if (retryAfterHeader is not null && retryAfterHeader.Value is not null)
                    {
                        var seconds = Int32.Parse(retryAfterHeader.Value);
                        var minutes = seconds / 60;
                        var durationPart = minutes == 0 ? $"{seconds} seconds." : $"{minutes} minutes.";
                        errDetails = $"This invoice has been sent within the last 10 minutes. Try again in {durationPart}";
                    }
                }

                throw new ProblemDetailsException(
                    StatusCodes.Status422UnprocessableEntity,
                    "UNPROCESSABLE_CONTENT",
                    errDetails
                    );
            }


            SubmitDocumentsResponseDTO responseDTO = new SubmitDocumentsResponseDTO();

            if ((int)response.StatusCode == StatusCodes.Status202Accepted && response.Content != null)
            {
                SuccessfulResponseDTO? serializedResponse = new();

                try
                {
                    var options = new JsonSerializerOptions
                    {
                        PropertyNamingPolicy = JsonNamingPolicy.CamelCase,
                        PropertyNameCaseInsensitive = true
                    };

                    serializedResponse = JsonSerializer.Deserialize<SuccessfulResponseDTO>(response.Content, options);
                }
                catch (JsonException)
                {
                    throw;
                }

                if (serializedResponse is null)
                    throw new ProblemDetailsException(
                        statusCode: StatusCodes.Status500InternalServerError,
                        message: "SERIALIZATION_FAILED",
                        detail: "Could not serialize the response."
                        );
                else
                {
                    responseDTO.SuccessfulResponseDTO = serializedResponse;
                    responseDTO.StatusCode = (int)response.StatusCode;
                    responseDTO.IsSuccess = true;
                }

            }
            else
            {
                responseDTO.IsSuccess = false;
                responseDTO.StatusCode = (int)response.StatusCode;
                responseDTO.Message = response.ErrorMessage ?? "";
            }

            return Task.FromResult(responseDTO);
        }
    }
}
