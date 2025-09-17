using ETA.Integrator.Server.Dtos.ConsumerAPI.RecentDocuments;
using ETA.Integrator.Server.Dtos.ConsumerAPI.SubmitDocuments;
using ETA.Integrator.Server.Helpers;
using ETA.Integrator.Server.Interface.Services.Common;
using ETA.Integrator.Server.Models.Consumer.Response;
using ETA.Integrator.Server.Models.Core;
using ETA.Integrator.Server.Models.Provider;
using ETA.Integrator.Server.Models.Provider.Response;
using RestSharp;
using System.Collections.Generic;
using System.Net;
using System.Text.Json;

namespace ETA.Integrator.Server.Services.Common
{
    public class ResponseProcessorService : IResponseProcessorService
    {
        public Task<T> ProcessResponse<T>(RestResponse response) where T : new()
        {
            ValidateResponse(response);

            T serializedResponse = GenericHelpers.JsonDeserialize<T>(response.Content ?? "");

            if (typeof(T) == typeof(ProviderLoginResponseModel))
            {
                var obj = serializedResponse as ProviderLoginResponseModel;

                if (obj is not null && obj.IsError)
                    throw new ProblemDetailsException(
                        statusCode: StatusCodes.Status401Unauthorized,
                        message: "UNAUTHORIZED",
                        detail: String.IsNullOrEmpty(obj.Message) ? "Username or password incorrect" : obj.Message
                        );

            }

            return Task.FromResult(serializedResponse!);
        }
        private static void ValidateResponse(RestResponse response)
        {
            if (response is null || response.Content is null)
            {
                throw new ProblemDetailsException(
                    statusCode: StatusCodes.Status500InternalServerError,
                    message: response is null ? "RESPONSE_NULL" : "CONTENT_NULL",
                    detail: response is null ? "No response to process" : response.ErrorMessage ?? "Content of the response is not available for unknown reason"
                    );
            }

            if ((response.StatusCode == HttpStatusCode.OK || response.StatusCode == HttpStatusCode.Accepted) && response.IsSuccessStatusCode)
                return;
            else
            {
                var errDetail = response.Content ?? response.ErrorMessage ?? "Response failed";

                if (response.StatusCode == HttpStatusCode.Forbidden || response.StatusCode == HttpStatusCode.Unauthorized)
                {// FORBIDDEN OR UNAUHORIZED
                    errDetail = response.Content ?? response.ErrorMessage ?? ((int)response.StatusCode == StatusCodes.Status403Forbidden ? "Needs permissions to do the request" : "Unauthorized to do the request");

                    throw new ProblemDetailsException(
                        (int)response.StatusCode,
                        (int)response.StatusCode == StatusCodes.Status403Forbidden ? "NEED_PERMISSIONS" : "UNAUTHORIZED",
                        errDetail
                        );
                }

                if (response.StatusCode == HttpStatusCode.UnprocessableEntity)
                {// UNPROCESSABLE ENTITY
                    errDetail = "Unexpected error";

                    if (response.Headers is not null && response.Headers.Count > 0)
                    {
                        var retryAfterHeader = response.Headers
                            .FirstOrDefault(h => h.Name.Equals("Retry-After", StringComparison.OrdinalIgnoreCase));

                        if (retryAfterHeader is not null && retryAfterHeader.Value is not null)
                        {
                            var seconds = Int32.Parse(retryAfterHeader.Value);
                            var minutes = seconds / 60;
                            var durationPart = minutes == 0 ? $"{seconds} seconds." : $"{minutes} minutes.";
                            errDetail = $"This invoice has been sent within the last 10 minutes. Try again in {durationPart}";
                        }
                    }

                    throw new ProblemDetailsException(
                        StatusCodes.Status422UnprocessableEntity,
                        "UNPROCESSABLE_CONTENT",
                        errDetail
                        );
                }

                throw new ProblemDetailsException(
                    (int?)response.StatusCode ?? StatusCodes.Status500InternalServerError,
                    "RESPONSE_FAILED",
                    errDetail
                    );
            }
        }
    }
}
