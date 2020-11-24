namespace Business.RequestHandlers.Managers
{
    using Business.Configuration;
    using Business.RequestHandlers.Interfaces;
    using EnsureThat;
    using Microsoft.Extensions.Logging;
    using System.Collections.Generic;
    using System.Threading.Tasks;
    using ZTR.Framework.Business;
    using ZTR.Framework.Business.File.FileReaders;

    public class DeviceTypeManager : Manager, IDeviceTypeManager
    {
        private readonly IGitRepositoryManager _repoManager;
        private readonly IGitConnectionOptionsFactory _gitFactoryManager;

        public DeviceTypeManager(ILogger<DeviceTypeManager> logger, IGitRepositoryManager gitRepoManager, IGitConnectionOptionsFactory gitFactoryManager) : base(logger)
        {
            EnsureArg.IsNotNull(logger, nameof(logger));
            EnsureArg.IsNotNull(gitFactoryManager, nameof(gitFactoryManager));

            _repoManager = gitRepoManager;
            _gitFactoryManager = gitFactoryManager;

            SetConnectionOptions();
        }

        public void SetConnectionOptions()
        {
            var connectionOptions = _gitFactoryManager.GetGitConnectionOption(GitConnectionOptionType.Device);
            _repoManager.SetConnectionOptions(connectionOptions);
        }

        public async Task<IEnumerable<string>> GetAllDevicesAsync()
        {
            var listOfDevices = new List<string>();

            await _repoManager.CloneRepositoryAsync();
            var deviceGitConnection = (DeviceGitConnectionOptions)_repoManager.GetConnectionOptions();

            if (deviceGitConnection != null)
            {
                listOfDevices = FileReaderExtensions.GetDirectories(deviceGitConnection.TomlConfiguration.DeviceFolder);
                listOfDevices = listOfDevices.ConvertAll(item => item.ToUpper());
            }

            return listOfDevices;
        }

        public async Task<IEnumerable<string>> GetAllFirmwareVersionsAsync()
        {
            var listFirmwareVersions = await _repoManager.LoadTagNamesAsync();

            return listFirmwareVersions;
        }
    }
}
