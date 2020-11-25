namespace Business.RequestHandlers.Interfaces
{
    using System.Threading.Tasks;

    public interface IDefaultValueManager
    {
        Task GetDefaultValuesAllModulesAsync(string firmwareVersion, string deviceType);
    }
}
