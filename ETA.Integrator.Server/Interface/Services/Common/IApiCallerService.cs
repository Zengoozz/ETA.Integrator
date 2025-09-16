using ETA.Integrator.Server.Dtos;
using ETA.Integrator.Server.Dtos.ConsumerAPI.RecentDocuments;
using ETA.Integrator.Server.Dtos.ConsumerAPI.Submission;
using ETA.Integrator.Server.Dtos.ConsumerAPI.SearchDocuments;
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
        Task<RecentDocumentsResponseDTO> GetRecentDocuments();
        Task<List<ProviderInvoiceViewModel>> GetProviderInvoices(DateTime? fromDate, DateTime? toDate, string invoiceType);
        Task<SubmissionResponseDTO> GetSubmission(string submissionId, int pageNumber = 5, int pageSize = 10);
        Task<SearchDocumentsResponseDTO> SearchDocuments(DateTime submissionDateFrom, DateTime submissionDateTo);

    }
}
