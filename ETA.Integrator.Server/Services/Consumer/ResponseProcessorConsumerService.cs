using ETA.Integrator.Server.Dtos.ConsumerAPI.GetRecentDocuments;
using ETA.Integrator.Server.Dtos.ConsumerAPI.SubmitDocuments;
using ETA.Integrator.Server.Interface.Services.Consumer;
using ETA.Integrator.Server.Models.Core;
using RestSharp;
using System.Text.Json;

namespace ETA.Integrator.Server.Services.Consumer
{
    public class ResponseProcessorConsumerService : IResponseProcessorConsumerService
    {
        public Task<GetRecentDocumentsResponseDTO> GetRecentDocuments(RestResponse response)
        {
            if (response is null || !response.IsSuccessful || (int)response.StatusCode != StatusCodes.Status200OK || response.Content is null)
            {
                if (response is null)
                    throw new ProblemDetailsException(
                        statusCode: StatusCodes.Status500InternalServerError,
                        message: "BAD_PARAMS",
                        detail: "ResponseProcessorConsumerService/GetRecentDocuments: No response to process."
                        );

                else if ((int)response.StatusCode == StatusCodes.Status403Forbidden)
                    throw new ProblemDetailsException(
                        statusCode: StatusCodes.Status403Forbidden,
                        message: "CONSUMER_FAILURE",
                        detail: response.ErrorMessage ?? response.Content ?? ""
                        );

                else
                    throw new ProblemDetailsException(
                        statusCode: StatusCodes.Status500InternalServerError,
                        message: "UNKNOWN",
                        detail: response.ErrorMessage ?? response.Content ?? ""
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
                        message: "SERIALIZATION_FAILED",
                        detail: "ResponseProcessorConsumerService/GetRecentDocuments: Could not serialize the response."
                        );

                return Task.FromResult(serializedResponse);
            }
        }

        
        public Task<SubmitDocumentsResponseDTO> SubmitDocuments(RestResponse response)
        {
            if (response is null)
                throw new ProblemDetailsException(
                    statusCode: StatusCodes.Status500InternalServerError,
                    message: "BAD_PARAMS",
                    detail: "ResponseProcessorConsumerService/SubmitDocuments: No response to process."
                    );

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
                        message: "SERIALIZATION_FAILED",
                        detail: "ResponseProcessorConsumerService/SubmitDocuments: Could not serialize the response."
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
