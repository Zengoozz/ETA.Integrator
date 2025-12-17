using ETA.Integrator.Server.Dtos.ConsumerAPI.GetDocument;
using ETA.Integrator.Server.Dtos.ConsumerAPI.Submission;
using ETA.Integrator.Server.Models.Consumer.ETA;

namespace ETA.Integrator.Server.Dtos.ConsumerAPI
{
    public class DocumentExtendedDTO : SubmissionSummaryDTO
    {
        public string SubmissionUUID { get; set; } = string.Empty;
        public DateTime? DateTimeReceived { get; set; }
        public decimal TotalSales { get; set; }
        public decimal TotalDiscount { get; set; }
        public decimal NetAmount { get; set; }
        public string TransformationStatus { get; set; } = string.Empty;
        public dynamic? Document { get; set; }
        public DocumentValidationResultsDTO ValidationResults { get; set; } = new DocumentValidationResultsDTO();
        public List<AdditionalMetadata> AdditionalMetadata { get; set; } = new List<AdditionalMetadata>();
    }
    public class AdditionalMetadata
    {
        public string FieldName { get; set; } = string.Empty;
        public string FieldValue { get; set; } = string.Empty;
        public string FieldType { get; set; } = string.Empty;
        public string FieldNameDescEn { get; set; } = string.Empty;
        public string FieldNameDescAr { get; set; } = string.Empty;
    }
}
