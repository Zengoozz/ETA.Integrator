namespace ETA.Integrator.Server.Dtos.ConsumerAPI.SubmitDocuments
{
    public class SubmitDocumentsResponseDTO
    {
        public int StatusCode {  get; set; }    
        public bool IsSuccess { get; set; }
        public string Message { get; set; } = string.Empty;
        public SuccessfulResponseDTO SuccessfulResponseDTO { get; set; } = new();
    }
}
