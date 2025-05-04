namespace ETA.Integrator.Server.Models.Consumer.Response
{
    public class GenericResponse<T>
    {
        public int? StatusCode { get; set; } = null;
        public bool Success { get; set; } = false;
        public string Message { get; set; } = string.Empty;
        public string Code { get; set; } = string.Empty;
        public T? Data { get; set; } = default(T);
    }
}
