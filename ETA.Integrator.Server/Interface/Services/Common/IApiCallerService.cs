using ETA.Integrator.Server.Dtos;
using ETA.Integrator.Server.Dtos.ConsumerAPI.GetRecentDocuments;
using ETA.Integrator.Server.Dtos.ConsumerAPI.GetSubmission;
using ETA.Integrator.Server.Dtos.ConsumerAPI.SubmitDocuments;
using ETA.Integrator.Server.Models;
using ETA.Integrator.Server.Models.Consumer.Response;
using ETA.Integrator.Server.Models.Provider;
using ETA.Integrator.Server.Models.Provider.Requests;
using ETA.Integrator.Server.Models.Provider.Response;

namespace ETA.Integrator.Server.Interface.Services.Common
{
    public interface IApiCallerService
    {
        Task<ProviderLoginResponseModel> ConnectToProvider(ProviderLoginRequestModel model);
        Task<ConsumerConnectionResponseModel> ConnectToConsumer(ConnectionDTO? model = null);
        Task<SubmitDocumentsResponseDTO> SubmitDocuments(InvoiceRequest request);
        Task<GetRecentDocumentsResponseDTO> GetRecentDocuments();
        Task<List<ProviderInvoiceViewModel>> GetProviderInvoices(DateTime? fromDate, DateTime? toDate, string invoiceType);
        Task<List<GetSubmissionResponseDTO>> GetSubmission(string uuid, int pageNumber = 5, int pageSize = 10);
        Task<GetRecentDocumentsResponseDTO> SearchDocuments(DateTime submissionDateFrom, DateTime submissionDateTo);

    }
}
