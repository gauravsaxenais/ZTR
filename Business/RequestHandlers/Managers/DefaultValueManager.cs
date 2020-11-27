namespace Business.RequestHandlers.Managers
{
    using Business.Configuration;
    using Business.Models;
    using Business.Parsers;
    using Business.Parsers.Models;
    using Business.RequestHandlers.Interfaces;
    using EnsureThat;
    using Microsoft.Extensions.Logging;
    using Newtonsoft.Json.Linq;
    using System;
    using System.Collections.Generic;
    using System.IO;
    using System.Linq;
    using System.Threading.Tasks;
    using ZTR.Framework.Business;
    using ZTR.Framework.Business.File.FileReaders;

    /// <summary>
    /// This class returns all the modules, their name and uuid information.
    /// It also returns of the default values for all the modules.
    /// It integrates with module parser to parse a proto file,
    /// recieves all the default values from default.toml and 
    /// receives the module.proto from corresponding module name
    /// and uuid folder.
    /// </summary>
    /// <seealso cref="ZTR.Framework.Business.Manager" />
    /// <seealso cref="Business.RequestHandlers.Interfaces.IDefaultValueManager" />
    public class DefaultValueManager : Manager, IDefaultValueManager
    {
        private readonly IGitRepositoryManager _gitRepoManager;
        private readonly DeviceGitConnectionOptions _deviceGitConnectionOptions;

        /// <summary>
        /// Initializes a new instance of the <see cref="DefaultValueManager"/> class.
        /// </summary>
        /// <param name="logger">The logger.</param>
        /// <param name="gitRepoManager">The git repo manager.</param>
        /// <param name="deviceGitConnectionOptions">The device git connection options.</param>
        public DefaultValueManager(ILogger<ModuleManager> logger, IGitRepositoryManager gitRepoManager, DeviceGitConnectionOptions deviceGitConnectionOptions) : base(logger)
        {
            EnsureArg.IsNotNull(logger, nameof(logger));
            EnsureArg.IsNotNull(deviceGitConnectionOptions, nameof(deviceGitConnectionOptions));

            _gitRepoManager = gitRepoManager;
            _deviceGitConnectionOptions = deviceGitConnectionOptions;

            var currentDirectory = Path.GetDirectoryName(System.Reflection.Assembly.GetExecutingAssembly().Location);

            _deviceGitConnectionOptions.GitLocalFolder = Path.Combine(currentDirectory, _deviceGitConnectionOptions.GitLocalFolder);

            _gitRepoManager.SetConnectionOptions(_deviceGitConnectionOptions);
        }

        /// <summary>
        /// Gets the default values for all modules in asynchronous fashion.
        /// </summary>
        /// <param name="firmwareVersion">The firmware version.</param>
        /// <param name="deviceType">Type of the device.</param>
        /// <returns></returns>
        public async Task<IEnumerable<ModuleReadModel>> GetDefaultValuesAllModulesAsync(string firmwareVersion, string deviceType)
        {
            var moduleFilePath = @"ProtoFiles\modules\";

            var inputFileLoader = new InputFileLoader();

            moduleFilePath = inputFileLoader.CombinePathFromAppRoot(moduleFilePath);

            var customMessageParser = new CustomMessageParser();
            var moduleParser = new ModuleParser();

            var tomlSettings = TomlFileReader.LoadLowerCaseTomlSettingsWithMappingForDefaultValues();

            var listOfDefaultValues = await GetDefaultData(firmwareVersion, deviceType);
            var listOfModules = await GetListOfModulesAsync(firmwareVersion, deviceType);
            var protoFilePaths = GetProtoFilePath(moduleFilePath, listOfModules);

            var messages = new List<CustomIMessage>();
            foreach (var filePath in protoFilePaths)
            {
                var fileName = Path.GetFileName(filePath.Value);
                
                string protoDirectory = new FileInfo(filePath.Value).Directory.FullName;

                var message = inputFileLoader.GenerateCodeFiles(fileName, protoDirectory);

                if (message != null)
                {
                    var customIMessage = new CustomIMessage();
                    customIMessage.Name = filePath.Key;
                    customIMessage.Message = message;
                    messages.Add(customIMessage);
                }
            }

            foreach (var message in messages)
            {
                var formattedMessage = customMessageParser.Format(message.Message);
                formattedMessage.Name = message.Name;

                var jsonContent = moduleParser.ReadFileAsJson(listOfDefaultValues, tomlSettings, formattedMessage);

                var module = listOfModules.Where(p => p.Name?.IndexOf(formattedMessage.Name, StringComparison.OrdinalIgnoreCase) >= 0).FirstOrDefault();

                module.Attributes = formattedMessage;

                if (module != null)
                {
                    if (string.IsNullOrWhiteSpace(jsonContent))
                    {
                        module.Attributes = formattedMessage;
                    }
                    else
                    {
                        module.Attributes = JObject.Parse(jsonContent);
                    }
                }
            }

            return listOfModules;
        }

        /// <summary>
        /// Gets the proto file path.
        /// </summary>
        /// <param name="moduleFilePath">The module file path.</param>
        /// <param name="listOfModules">The list of modules.</param>
        /// <returns></returns>
        private Dictionary<string, string> GetProtoFilePath(string moduleFilePath, IEnumerable<ModuleReadModel> listOfModules)
        {
            EnsureArg.IsNotNullOrWhiteSpace(moduleFilePath);
            EnsureArg.IsNotNull(listOfModules);

            var protoFilePath = new Dictionary<string, string>();

            if (listOfModules.Any())
            {
                foreach (var moduleName in listOfModules)
                {
                    var moduleFolder = FileReaderExtensions.GetSubDirectoryPath(moduleFilePath, moduleName.Name);

                    if(!string.IsNullOrWhiteSpace(moduleFolder))
                    {
                        var uuidFolder = FileReaderExtensions.GetSubDirectoryPath(moduleFolder, moduleName.UUID);

                        foreach (string file in Directory.EnumerateFiles(uuidFolder, "module.proto"))
                        {
                            protoFilePath.Add(moduleName.Name, file);
                        }
                    }
                }
            }

            return protoFilePath;
        }

        private async Task<string> GetDefaultData(string firmwareVersion, string deviceType)
        {
            var gitConnectionOptions = (DeviceGitConnectionOptions)_gitRepoManager.GetConnectionOptions();

            var listOfFiles = await _gitRepoManager.GetFileDataFromTagAsync(firmwareVersion, gitConnectionOptions.TomlConfiguration.DefaultTomlFile);

            // case insensitive search.
            var deviceTypeFile = listOfFiles.Where(p => p.FileName?.IndexOf(deviceType, StringComparison.OrdinalIgnoreCase) >= 0).FirstOrDefault();

            var fileContent = string.Empty;

            if (deviceTypeFile != null)
            {
                fileContent = System.Text.Encoding.UTF8.GetString(deviceTypeFile.Data);
            }

            return fileContent;
        }

        /// <summary>
        /// Gets the list of modules asynchronous.
        /// </summary>
        /// <param name="firmwareVersion">The firmware version.</param>
        /// <param name="deviceType">Type of the device.</param>
        /// <returns></returns>
        private async Task<IEnumerable<ModuleReadModel>> GetListOfModulesAsync(string firmwareVersion, string deviceType)
        {
            var listOfModules = new List<ModuleReadModel>();

            var fileContent = await GetDeviceDataFromFirmwareVersionAsync(firmwareVersion, deviceType);
            if (!string.IsNullOrWhiteSpace(fileContent))
            {
                var data = GetTomlData(fileContent);

                listOfModules = data.Module;
            }

            return listOfModules;
        }

        /// <summary>
        /// Gets the toml data.
        /// </summary>
        /// <param name="fileContent">Content of the file.</param>
        /// <returns></returns>
        private ConfigurationReadModel GetTomlData(string fileContent)
        {
            var tomlSettings = TomlFileReader.LoadLowerCaseTomlSettingsWithMappingForDefaultValues();

            var tomlData = TomlFileReader.ReadDataFromString<ConfigurationReadModel>(data: fileContent, settings: tomlSettings);

            return tomlData;
        }

        /// <summary>
        /// Gets the device data from firmware version asynchronous.
        /// </summary>
        /// <param name="firmwareVersion">The firmware version.</param>
        /// <param name="deviceType">Type of the device.</param>
        /// <returns></returns>
        private async Task<string> GetDeviceDataFromFirmwareVersionAsync(string firmwareVersion, string deviceType)
        {
            var gitConnectionOptions = (DeviceGitConnectionOptions)_gitRepoManager.GetConnectionOptions();

            var listOfFiles = await _gitRepoManager.GetFileDataFromTagAsync(firmwareVersion, gitConnectionOptions.TomlConfiguration.DeviceTomlFile);

            // case insensitive search.
            var deviceTypeFile = listOfFiles.Where(p => p.FileName?.IndexOf(deviceType, StringComparison.OrdinalIgnoreCase) >= 0).FirstOrDefault();

            var fileContent = string.Empty;

            if (deviceTypeFile != null)
            {
                fileContent = System.Text.Encoding.UTF8.GetString(deviceTypeFile.Data);
            }

            return fileContent;
        }
    }
}
