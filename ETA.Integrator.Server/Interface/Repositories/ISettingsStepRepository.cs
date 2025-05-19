using ETA.Integrator.Server.Entities;

namespace ETA.Integrator.Server.Interface.Repositories
{
    public interface ISettingsStepRepository
    {
        Task<List<SettingsStep>> GetAll();
        Task<SettingsStep?> GetByStepNumber(int stepNumber);
        Task<int> GetFirstUnCompletedStepOrder();
        Task UpdateStepWithData(int stepNumber, string data);
    }
}
