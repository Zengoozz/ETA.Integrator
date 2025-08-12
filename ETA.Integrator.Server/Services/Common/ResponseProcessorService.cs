using ETA.Integrator.Server.Dtos.ConsumerAPI.GetRecentDocuments;
using ETA.Integrator.Server.Dtos.ConsumerAPI.SubmitDocuments;
using ETA.Integrator.Server.Interface.Services.Common;
using ETA.Integrator.Server.Models.Core;
using ETA.Integrator.Server.Models.Provider;
using RestSharp;
using System.Text.Json;

namespace ETA.Integrator.Server.Services.Common
{
    public class ResponseProcessorService : IResponseProcessorService
    {
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
                catch (JsonException ex)
                {
                    Console.WriteLine("JSON Error: " + ex.Message);
                }
            }
            else
            {
                throw new ProblemDetailsException(
                   (int?)response?.StatusCode ?? StatusCodes.Status500InternalServerError,
                   "ResponseProcessorService/GetProviderInvoices: PROVIDER_SERVER_ERR",
                   response?.ErrorMessage ?? "Unexpected error"
                   );
            }

            if (serializedResponse is null)
                throw new ProblemDetailsException(
                    statusCode: StatusCodes.Status500InternalServerError,
                    message: "ResponseProcessorService/GetProviderInvoices: SERIALIZATION_FAILED",
                    detail: "Could not serialize the response."
                    );


            return Task.FromResult(serializedResponse);
        }

        public Task<GetRecentDocumentsResponseDTO> GetRecentDocuments(RestResponse response)
        {
            if (response is null || !response.IsSuccessful || (int)response.StatusCode != StatusCodes.Status200OK || response.Content is null)
            {
                if (response is null)
                    throw new ProblemDetailsException(
                        statusCode: StatusCodes.Status500InternalServerError,
                        message: "ResponseProcessorConsumerService/GetRecentDocuments: BAD_PARAMS",
                        detail: "No response to process."
                        );

                else if ((int)response.StatusCode == StatusCodes.Status403Forbidden)
                    throw new ProblemDetailsException(
                        statusCode: StatusCodes.Status403Forbidden,
                        message: "ResponseProcessorConsumerService/GetRecentDocuments: CONSUMER_FAILURE",
                        detail: response.ErrorMessage ?? response.Content ?? "Unexpected error"
                        );

                else
                    throw new ProblemDetailsException(
                        statusCode: StatusCodes.Status500InternalServerError,
                        message: "ResponseProcessorConsumerService/GetRecentDocuments: UNKNOWN",
                        detail: response.ErrorMessage ?? response.Content ?? "Unexpected error"
                        );

            }
            else
            {
                GetRecentDocumentsResponseDTO? serializedResponse = new GetRecentDocumentsResponseDTO();
                if (response != null && (int)response.StatusCode == StatusCodes.Status200OK && response.Content != null)
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
                    catch (JsonException ex)
                    {
                        Console.WriteLine("JSON Error: " + ex.Message);
                    }
                }

                if (serializedResponse is null)
                    throw new ProblemDetailsException(
                        statusCode: StatusCodes.Status500InternalServerError,
                        message: "ResponseProcessorService/GetRecentDocuments: SERIALIZATION_FAILED",
                        detail: "Could not serialize the response."
                        );

                return Task.FromResult(serializedResponse);
            }
        }
        public Task<SubmitDocumentsResponseDTO> SubmitDocuments(RestResponse response)
        {
            if (response is null)
                throw new ProblemDetailsException(
                    statusCode: StatusCodes.Status500InternalServerError,
                    message: "ResponseProcessorConsumerService/SubmitDocuments: BAD_PARAMS",
                    detail: "No response to process."
                    );

            if ((int)response.StatusCode == StatusCodes.Status422UnprocessableEntity)
            {
                var errDetails = "Unexpected error";

                if (response.Headers is not null && response.Headers.Count > 0)
                {
                    var retryAfterHeader = response.Headers
                        .FirstOrDefault(h => h.Name.Equals("Retry-After", StringComparison.OrdinalIgnoreCase));

                    if(retryAfterHeader is not null && retryAfterHeader.Value is not null)
                    {
                        var seconds = Int32.Parse(retryAfterHeader.Value);
                        var minutes = seconds / 60;
                        var durationPart = minutes == 0 ? $"{seconds} seconds." : $"{minutes} minutes.";
                        errDetails = $"This invoice has been sent within the last 10 minutes. Try again in {durationPart}";
                    }
                }

                throw new ProblemDetailsException(
                    StatusCodes.Status422UnprocessableEntity,
                    "ResponseProcessorConsumerService/SubmitDocuments: UNPROCESSABLE_CONTENT",
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
                catch (JsonException ex)
                {
                    Console.WriteLine("JSON Error: " + ex.Message);
                }

                if (serializedResponse is null)
                    throw new ProblemDetailsException(
                        statusCode: StatusCodes.Status500InternalServerError,
                        message: "ResponseProcessorService/SubmitDocuments: SERIALIZATION_FAILED",
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
