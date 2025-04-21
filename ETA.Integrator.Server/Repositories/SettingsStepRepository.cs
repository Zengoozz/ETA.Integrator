using ETA.Integrator.Server.Data;
using ETA.Integrator.Server.Entities;
using ETA.Integrator.Server.Interface.Repositories;
using Microsoft.EntityFrameworkCore;
using System.Linq;

namespace ETA.Integrator.Server.Repositories
{
    public class SettingsStepRepository : ISettingsStepRepository
    {
        private readonly AppDbContext _context;
        private readonly DbSet<SettingsStep> _dbSet;
        public SettingsStepRepository(AppDbContext context)
        {
            _context = context;
            _dbSet = context.Set<SettingsStep>();
        }
        public async Task UpdateStepWithData(int stepNumber, string data)
        {
            try
            {
                SettingsStep? settingsStep = await _dbSet.FirstOrDefaultAsync(t => t.Order == stepNumber);
                if (settingsStep != null)
                {
                    settingsStep.Data = data;
                    await _context.SaveChangesAsync();
                }
            }
            catch (Exception)
            {
                throw;
            }
        }

        public async Task<List<SettingsStep>> GetAll()
        {
            return await _dbSet.AsNoTracking().ToListAsync();
        }

        public async Task<int> GetFirstUnCompletedStepOrder()
        {
            SettingsStep? settingStep = await _dbSet.AsNoTracking().FirstOrDefaultAsync(t => t.Data == null) ?? null;

            if (settingStep == null)
                return -1;
            else
                return settingStep.Order;
        }

        public async Task<SettingsStep> GetByStepNumber(int stepNumber)
        {
            return await _dbSet.AsNoTracking().FirstOrDefaultAsync(t => t.Order == stepNumber) ?? new SettingsStep();
        }


    }
}
