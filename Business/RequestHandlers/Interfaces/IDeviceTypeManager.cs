namespace Business.RequestHandlers.Interfaces
{
    using System.Collections.Generic;
    using System.Threading.Tasks;

    public interface IDeviceTypeManager
    {
        Task<IEnumerable<string>> GetAllDevicesAsync();
        Task<IEnumerable<string>> GetAllFirmwareVersionsAsync();
    }
}
