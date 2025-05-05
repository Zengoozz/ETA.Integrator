using System.Net;

namespace ETA.Integrator.Server.BuildingBlocks
{
    public class Result
    {
        protected internal Result(bool isSuccess, Error error, HttpStatusCode statusCode)
        {
            if (isSuccess && error != Error.None)
                throw new InvalidOperationException();
            if (!isSuccess && error == Error.None)
                throw new InvalidOperationException();

            StatusCode = statusCode;
            IsSuccess = isSuccess;
            Error = error;
        }
        public bool IsSuccess { get; }
        public bool IsFailure => !IsSuccess;
        public Error Error { get; }
        public HttpStatusCode StatusCode { get; }

        public static Result Success() => new(true, Error.None, HttpStatusCode.OK);
        public static Result<TValue> Success<TValue>(TValue value) => new Result<TValue>(value, true, Error.None, HttpStatusCode.OK);
        public static Result Failure(Error error) => new(false, error, HttpStatusCode.BadRequest);
        public static Result Failure(Error error, HttpStatusCode code) => new(false, error, code);
        public static Result<TValue> Failure<TValue>(Error error) => new Result<TValue>(default, false, error, HttpStatusCode.BadRequest);
        public static Result<TValue> Failure<TValue>(string code, string message) => new Result<TValue>(default, false, new Error(code, message), HttpStatusCode.BadRequest);
        public static Result Created() => new(true, Error.None, HttpStatusCode.Created);
        public static Result<TValue> Created<TValue>(TValue? value) => value is not null ? Success(value) : Failure<TValue>(Error.NullValue);
        public static Result NotFound(string message = "The requested resource is not found.") => Failure(new Error("NotFound", message), HttpStatusCode.NotFound);
        public static Result<TValue> NotFound<TValue>(string message = "The requested resource is not found.") => new Result<TValue>(default, false, new Error("NotFound", message), HttpStatusCode.NotFound);
        public static Result BadRequest(string message = "The request is invalid.") => Failure(new Error("BadRequest", message), HttpStatusCode.BadRequest);
        public static Result<TValue> BadRequest<TValue>(string message = "The request is invalid.") => new(default, false, new Error("BadRequest", message), HttpStatusCode.BadRequest);
        public static Result InternalError(string message = "An internal server error occured.") => Failure(new Error("InternalError", message), HttpStatusCode.InternalServerError);
        public static Result<TValue> InternalError<TValue>(string message = "An internal server error occured.") => new(default, false, new Error("InternalError", message), HttpStatusCode.InternalServerError);
        public static Result Unauthorized(string message = "Unauthorized access.") => Failure(new Error("Unauthorized", message), HttpStatusCode.Unauthorized);
    }
}
