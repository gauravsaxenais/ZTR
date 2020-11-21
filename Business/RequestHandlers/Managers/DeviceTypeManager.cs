namespace Business.RequestHandlers.Managers
{
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
        
        public DeviceTypeManager(ILogger<DeviceTypeManager> logger, IGitRepositoryManager gitRepoManager, DeviceGitConnectionOptions gitConnectionOptions) : base(logger)
        {
            EnsureArg.IsNotNull(logger, nameof(logger));
            EnsureArg.IsNotNull(gitConnectionOptions, nameof(gitConnectionOptions));
            EnsureArg.IsNotNull(gitConnectionOptions.TomlConfiguration, nameof(gitConnectionOptions.TomlConfiguration));

            var currentDirectory = Path.GetDirectoryName(System.Reflection.Assembly.GetExecutingAssembly().Location);
            gitConnectionOptions.GitLocalFolder = Path.Combine(currentDirectory, gitConnectionOptions.GitLocalFolder);
            gitConnectionOptions.TomlConfiguration.DeviceFolder = Path.Combine(gitConnectionOptions.GitLocalFolder, gitConnectionOptions.TomlConfiguration.DeviceFolder);

            _repoManager = gitRepoManager;
            _repoManager.SetConnectionOptions(gitConnectionOptions);
        }

        public async Task<IEnumerable<string>> GetAllDevicesAsync()
        {
            var listOfDevices = new List<string>();

            await _repoManager.CloneRepositoryAsync();
            var gitConnection = _repoManager.GetConnectionOptions();

            if (gitConnection != null)
            {
                listOfDevices = FileReaderExtensions.GetDirectories(gitConnection.TomlConfiguration.DeviceFolder);
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
