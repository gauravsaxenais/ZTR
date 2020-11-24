namespace Business.RequestHandlers.Managers
{
    using Business.Configuration;
    using Business.RequestHandlers.Interfaces;
    using EnsureThat;
    using Microsoft.Extensions.Logging;
    using System.Collections.Generic;
    using System.IO;
    using System.Threading.Tasks;
    using ZTR.Framework.Business;
    using ZTR.Framework.Business.File.FileReaders;

    public class DeviceTypeManager : Manager, IDeviceTypeManager
    {
        private readonly IGitRepositoryManager _repoManager;
        private readonly DeviceGitConnectionOptions _deviceGitConnectionOptions;

        public DeviceTypeManager(ILogger<DeviceTypeManager> logger, IGitRepositoryManager gitRepoManager, DeviceGitConnectionOptions deviceGitConnectionOptions) : base(logger)
        {
            EnsureArg.IsNotNull(logger, nameof(logger));
            EnsureArg.IsNotNull(deviceGitConnectionOptions, nameof(deviceGitConnectionOptions));

            _repoManager = gitRepoManager;
            _deviceGitConnectionOptions = deviceGitConnectionOptions;

            var currentDirectory = Path.GetDirectoryName(System.Reflection.Assembly.GetExecutingAssembly().Location);

            _deviceGitConnectionOptions.GitLocalFolder = Path.Combine(currentDirectory, _deviceGitConnectionOptions.GitLocalFolder);
            _deviceGitConnectionOptions.TomlConfiguration.DeviceFolder = Path.Combine(_deviceGitConnectionOptions.GitLocalFolder, _deviceGitConnectionOptions.TomlConfiguration.DeviceFolder);

            _repoManager.SetConnectionOptions(_deviceGitConnectionOptions);
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
