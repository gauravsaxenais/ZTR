namespace Business.RequestHandlers.Interfaces
{
    using System.Threading.Tasks;
    using ZTR.Framework.Business;

    /// <summary>
    /// This Manager returns a list of all compatible firmware versions.
    /// </summary>
    public interface ICompatibleFirmwareVersionManager
    {
        /// <summary>
        /// Gets the compatible firmware versions asynchronous.
        /// </summary>
        /// <param name="firmwareVersion">The firmware version.</param>
        /// <param name="deviceType">Type of the device.</param>
        /// <returns></returns>
        Task<ApiResponse> GetCompatibleFirmwareVersionsAsync(string firmwareVersion, string deviceType);
    }
}
