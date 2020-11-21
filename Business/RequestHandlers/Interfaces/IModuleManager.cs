namespace Business.RequestHandlers.Interfaces
{
    using Business.Models;
    using System.Collections.Generic;
    using System.Threading.Tasks;

    public interface IModuleManager
    {
        Task<IEnumerable<ModuleReadModel>> GetAllModulesAsync(string firmwareVersion, string deviceType);
        Task<IEnumerable<ModuleReadModel>> GetModelByNameAsync(string firmwareVersion, string deviceType, IEnumerable<string> names);
        Task<IEnumerable<ModuleReadModel>> GetModuleByNameAsync(string name, string firmwareVersion, string deviceType, params string[] names);
    }
}
