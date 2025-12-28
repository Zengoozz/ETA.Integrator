using ETA.Integrator.Server.Dtos;
using ETA.Integrator.Server.Entities;
using ETA.Integrator.Server.Helpers.Enums;

namespace ETA.Integrator.Server.Interface.Repositories
{
    public interface IInvoiceSubmissionLogRepository
    {
        Task<List<InvoiceSubmissionLog>> GetAll();
        Task<List<InvoiceSubmissionLog>> GetAllValidWithIds(List<string> invoicesIds);
        Task<InvoiceSubmissionLog?> GetById(int id);
        Task<List<InvoiceSubmissionLog>> GetByInternalId(string id);
        Task<List<InvoiceSubmissionLog>> GetByListOfInternalIds(List<string> ids);
        Task<InvoiceSubmissionLog> GetValidByInternalId(string internalId);
        Task<List<InvoiceSubmissionLog>> GetByListOfUuids(List<string> uuids);
        Task Save(InvoiceSubmissionLog entity);
        Task SaveList(List<InvoiceSubmissionLog> listOfEntities);
        Task UpdateStatus(int id, InvoiceStatus status);
        Task UpdateListOfSubmissionsStatus(List<UpdateSubmissionStatusDTO> submissionsToUpdate);
        Task<List<InvoiceSubmissionLog>> GetUnvalidatedSubmissions();
        Task<(List<InvoiceSubmissionLog> submitted, List<InvoiceSubmissionLog> valid)> GetValidAndSubmittedByInternalId(List<string> internalIds);
        Task UpdateStatusWithListOfIds(List<int> listOfIds, InvoiceStatus status);
        Task<List<InvoiceSubmissionLog>> GetForOnlySubmittedByInternalIdDescOrdered(string internalId);
        Task UpdateLog(InvoiceSubmissionLog log);
    }
}
