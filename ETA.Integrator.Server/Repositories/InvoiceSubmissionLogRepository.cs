using ETA.Integrator.Server.Data;
using ETA.Integrator.Server.Entities;
using ETA.Integrator.Server.Helpers.Enums;
using ETA.Integrator.Server.Interface.Repositories;
using Microsoft.EntityFrameworkCore;

namespace ETA.Integrator.Server.Repositories
{
    public class InvoiceSubmissionLogRepository: IInvoiceSubmissionLogRepository
    {
        private readonly AppDbContext _context;
        private readonly DbSet<InvoiceSubmissionLog> _dbSet;
        public InvoiceSubmissionLogRepository(AppDbContext context)
        {
            _context = context;
            _dbSet = context.Set<InvoiceSubmissionLog>();
        }

        public async Task<List<InvoiceSubmissionLog>> GetAll()
        {
            return await _dbSet.AsNoTracking().ToListAsync();
        }

        public async Task<List<InvoiceSubmissionLog>> GetAllValidWithIds(List<string> invoicesIds)
        {
            return await _dbSet.AsNoTracking().Where(l => invoicesIds.Contains(l.InternalId) && l.Status >= InvoiceStatus.Submitted).ToListAsync();
        }

        public async Task<InvoiceSubmissionLog?> GetById(int id)
        {
            return await _dbSet.AsNoTracking().FirstOrDefaultAsync(x => x.Id == id);
        }

        public async Task<List<InvoiceSubmissionLog>> GetByInternalId(int id)
        {
            return await _dbSet.AsNoTracking().Where(x => x.InternalId == id.ToString()).ToListAsync();
        }

        public async Task<List<InvoiceSubmissionLog>> GetByListOfInternalIds(List<string> ids)
        {
            return await _dbSet.AsNoTracking().Where(x => ids.Contains(x.InternalId)).ToListAsync();
        }

        public async Task Save(InvoiceSubmissionLog entity)
        {
            try
            {
                var existed = await _dbSet.FirstOrDefaultAsync(x => x.Id == entity.Id);

                if(existed != null)
                {
                    _context.Entry(existed).CurrentValues.SetValues(entity);
                    _context.Entry(existed).Property(x => x.Id).IsModified = false;
                }
                else
                    await _context.AddAsync(entity);

                await _context.SaveChangesAsync();
            }
            catch (Exception) {
                throw;
            }
        }

        public async Task SaveList(List<InvoiceSubmissionLog> listOfEntities)
        {
            try
            {
                _context.UpdateRange(listOfEntities);
                await _context.SaveChangesAsync();
            }
            catch
            {
                throw;
            }
        }

        public async Task UpdateStatus(int id, InvoiceStatus status)
        {
            try
            {
                var existed = await _dbSet.FirstOrDefaultAsync(x => x.Id == id);

                if (existed != null)
                {
                    existed.Status = status;
                    existed.StatusStringfied = status.ToString();
                    await _context.SaveChangesAsync();
                }
                else
                    throw new Exception("NOT_FOUND");
            }
            catch (Exception) {
                throw;
            }
        }
    }
}
