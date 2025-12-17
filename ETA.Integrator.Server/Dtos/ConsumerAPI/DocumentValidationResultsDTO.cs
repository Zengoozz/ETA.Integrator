
namespace ETA.Integrator.Server.Dtos.ConsumerAPI.GetDocument
{
    public class DocumentValidationResultsDTO
    {
        public string Status { get; set; } = string.Empty;
        public List<ValidationStepResultDTO> ValidationSteps { get; set; } = new();
    }
    public class ValidationStepResultDTO
    {
        public string Name { get; set; } = string.Empty;
        public string status { get; set; } = string.Empty;
        public ErrorDTO? Error { get; set; }
    }
}
