using ETA.Integrator.Server.Dtos.ConsumerAPI.GetRecentDocuments;
using ETA.Integrator.Server.Interface.Services.Consumer;
using ETA.Integrator.Server.Models.Core;
using RestSharp;
using System.Text.Json;
using System.Text.Json.Serialization;

namespace ETA.Integrator.Server.Services.Consumer
{
    public class ResponseProcessorConsumerService : IResponseProcessorConsumerService
    {
        public Task<ResponseDTO> GetRecentDocuments(RestResponse response)
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
                ResponseDTO? serializedResponse = new ResponseDTO();
                if (response != null && (int)response.StatusCode == StatusCodes.Status200OK && response.Content != null)
                {
                    //TODO: Handle JSON Deserialize Exceptions
                    try
                    {
                        var options = new JsonSerializerOptions
                        {
                            PropertyNamingPolicy = JsonNamingPolicy.CamelCase,
                            PropertyNameCaseInsensitive = true
                        };
                        serializedResponse = JsonSerializer.Deserialize<ResponseDTO>(response.Content, options);
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

        public Task SubmitInvoices(RestResponse response)
        {
            throw new NotImplementedException();
        }
    }
}
