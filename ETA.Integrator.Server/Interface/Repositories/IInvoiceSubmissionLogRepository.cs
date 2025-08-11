using ETA.Integrator.Server.Entities;
using ETA.Integrator.Server.Helpers.Enums;

namespace ETA.Integrator.Server.Interface.Repositories
{
    public interface IInvoiceSubmissionLogRepository
    {
        Task<List<InvoiceSubmissionLog>> GetAll();
        Task<InvoiceSubmissionLog?> GetById(int id);
        Task<List<InvoiceSubmissionLog>> GetByInternalId(int id);
        Task Save(InvoiceSubmissionLog entity);
        Task SaveList(List<InvoiceSubmissionLog> listOfEntities);
        Task UpdateStatus(int id, InvoiceStatus status);
    }
}
