namespace Business.RequestHandlers.Managers
{
    using Business.Configuration;
    using Business.Models;
    using Business.Parsers.ProtoParser.Parser;
    using Business.RequestHandlers.Interfaces;
    using EnsureThat;
    using Microsoft.Extensions.Logging;
    using Nett;
    using System;
    using System.Collections.Concurrent;
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
    /// <seealso cref="Manager" />
    /// <seealso cref="IDefaultValueManager" />
    public class DefaultValueManager : Manager, IDefaultValueManager
    {
        private readonly IGitRepositoryManager _gitRepoManager;
        private readonly DeviceGitConnectionOptions _deviceGitConnectionOptions;
        private readonly string protoFileName = "module.proto";
        private readonly IProtoMessageParser _protoParser;
        private readonly ICustomMessageParser _customMessageParser;
        private readonly IModuleParser _moduleParser;

        /// <summary>
        /// Initializes a new instance of the <see cref="DefaultValueManager"/> class.
        /// </summary>
        /// <param name="logger">The logger.</param>
        /// <param name="gitRepoManager">The git repo manager.</param>
        /// <param name="deviceGitConnectionOptions">The device git connection options.</param>
        /// <param name="protoParser">The proto parser.</param>
        /// <param name="customMessageParser">The custom message parser.</param>
        /// <param name="moduleParser">The module parser.</param>
        public DefaultValueManager(ILogger<DefaultValueManager> logger, 
                                    IGitRepositoryManager gitRepoManager,
                                    DeviceGitConnectionOptions deviceGitConnectionOptions, 
                                    IProtoMessageParser protoParser,
                                    ICustomMessageParser customMessageParser,
                                    IModuleParser moduleParser) : base(logger)
        {
            EnsureArg.IsNotNull(logger, nameof(logger));
            EnsureArg.IsNotNull(gitRepoManager, nameof(gitRepoManager));
            EnsureArg.IsNotNull(deviceGitConnectionOptions, nameof(deviceGitConnectionOptions));
            EnsureArg.IsNotNull(deviceGitConnectionOptions.DefaultTomlConfiguration, nameof(deviceGitConnectionOptions.DefaultTomlConfiguration));
            EnsureArg.IsNotNull(protoParser, nameof(protoParser));
            EnsureArg.IsNotNull(customMessageParser, nameof(customMessageParser));
            EnsureArg.IsNotNull(moduleParser, nameof(moduleParser));

            _gitRepoManager = gitRepoManager;
            _protoParser = protoParser;
            _customMessageParser = customMessageParser;
            _moduleParser = moduleParser;
            _deviceGitConnectionOptions = deviceGitConnectionOptions;
        }

        /// <summary>
        /// Gets the default values for all modules in asynchronous fashion.
        /// </summary>
        /// <param name="firmwareVersion">The firmware version.</param>
        /// <param name="deviceType">Type of the device.</param>
        /// <returns></returns>
        public async Task<IEnumerable<ModuleReadModel>> GetDefaultValuesAllModulesAsync(string firmwareVersion, string deviceType)
        {
            SetConnection();

            // 1. get list of modules based on their firmware version and device type.
            // 2. get protofile paths based on firmware version and device type.
            // 3. create custom message for each of protofiles.
            // 4. get list of modules and their custom messages.
            var modulesProtoFolder = _deviceGitConnectionOptions.ModulesConfig;

            // read default values from toml file defaults.toml
            var defaultValueFromTomlFile = await GetFileContentFromPath(firmwareVersion, deviceType, _deviceGitConnectionOptions.DefaultTomlConfiguration.DefaultTomlFile);
            var deviceDataFromTomlFile = await GetFileContentFromPath(firmwareVersion, deviceType, _deviceGitConnectionOptions.DefaultTomlConfiguration.DeviceTomlFile);

            // get list of all modules.
            var listOfModules = GetListOfModules(deviceDataFromTomlFile);

            await MergeValuesWithModulesAsync(defaultValueFromTomlFile, listOfModules, modulesProtoFolder);

            return listOfModules;
        }

        /// <summary>
        /// Merges the values with modules asynchronous.
        /// </summary>
        /// <param name="defaultValueFromTomlFile">The default value from toml file.</param>
        /// <param name="listOfModules">The list of modules.</param>
        /// <param name="modulesProtoFolder">The modules proto folder.</param>
        public async Task MergeValuesWithModulesAsync(string defaultValueFromTomlFile, IEnumerable<ModuleReadModel> listOfModules, string modulesProtoFolder)
        {
            var degreeOfParallelism = 10;

            await Task.WhenAll(
                from partition in Partitioner.Create(listOfModules).GetPartitions(degreeOfParallelism)
                select Task.Run(async delegate
                {
                    using (partition)
                        while (partition.MoveNext())
                            await MergeDefaultValuesWithModules(defaultValueFromTomlFile, partition.Current, modulesProtoFolder);
                }));
        }

        private async Task MergeDefaultValuesWithModules(string defaultValueFromTomlFile, ModuleReadModel module, string modulesProtoFolder)
        {
            // get proto files for corresponding module and their uuid
            var protoFilePath = GetProtoFiles(modulesProtoFolder, module);

            if (!string.IsNullOrWhiteSpace(protoFilePath))
            {
                // get protoparsed messages from the proto files.
                var message = await _protoParser.GetCustomMessages(protoFilePath).ConfigureAwait(false);

                var formattedMessage = _customMessageParser.Format(message.Message);
                formattedMessage.Name = module.Name;

                var configValues = GetConfigValues(defaultValueFromTomlFile, module.Name);

                if (configValues != null && configValues.Any())
                {
                    var jsonModels = _moduleParser.MergeTomlWithProtoMessage(configValues, formattedMessage);
                    module.Config = jsonModels;
                }
            }
        }

        private Dictionary<string, object> GetConfigValues(string fileContent, string moduleName)
        {
            var tomlSettings = TomlFileReader.LoadLowerCaseTomlSettingsWithMappingForDefaultValues();
            var fileData = Toml.ReadString(fileContent, tomlSettings);

            var dictionary = fileData.ToDictionary();
            var listOfModules = (Dictionary<string, object>[])dictionary["module"];

            // here message.name means Power, j1939 etc.
            var module = listOfModules.Where(dic => dic.Values.Contains(moduleName)).FirstOrDefault();

            var configValues = new Dictionary<string, object>();

            if (module != null)
            {
                if (module.ContainsKey("config"))
                {
                    configValues = (Dictionary<string, object>)module["config"];
                }
            }

            return configValues;
        }

        private void SetConnection()
        {
            var currentDirectory = Path.GetDirectoryName(System.Reflection.Assembly.GetExecutingAssembly().Location);

            _deviceGitConnectionOptions.GitLocalFolder = Path.Combine(currentDirectory, _deviceGitConnectionOptions.GitLocalFolder);
            _deviceGitConnectionOptions.DefaultTomlConfiguration.DeviceFolder = Path.Combine(_deviceGitConnectionOptions.GitLocalFolder, _deviceGitConnectionOptions.DefaultTomlConfiguration.DeviceFolder);
            _deviceGitConnectionOptions.ModulesConfig = Path.Combine(currentDirectory, _deviceGitConnectionOptions.GitLocalFolder, _deviceGitConnectionOptions.ModulesConfig);

            _gitRepoManager.SetConnectionOptions(_deviceGitConnectionOptions);
        }

        /// <summary>
        /// Gets the proto files.
        /// </summary>
        /// <param name="moduleFilePath">The module file path.</param>
        /// <param name="moduleName">Name of the module.</param>
        /// <returns></returns>
        private string GetProtoFiles(string moduleFilePath, ModuleReadModel moduleName)
        {
            EnsureArg.IsNotNullOrWhiteSpace(moduleFilePath);

            var moduleFolder = FileReaderExtensions.GetSubDirectoryPath(moduleFilePath, moduleName.Name);

            if (!string.IsNullOrWhiteSpace(moduleFolder))
            {
                var uuidFolder = FileReaderExtensions.GetSubDirectoryPath(moduleFolder, moduleName.UUID);

                if (!string.IsNullOrWhiteSpace(uuidFolder))
                {
                    foreach (string file in Directory.EnumerateFiles(uuidFolder, protoFileName))
                    {
                        return file;
                    }
                }
            }

            return string.Empty;
        }

        private async Task<string> GetFileContentFromPath(string firmwareVersion, string deviceType, string path)
        {
            var gitConnectionOptions = (DeviceGitConnectionOptions)_gitRepoManager.GetConnectionOptions();

            var listOfFiles = await _gitRepoManager.GetFileDataFromTagAsync(firmwareVersion, path)
                                                   .ConfigureAwait(false);

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
        /// Gets the list of modules.
        /// </summary>
        /// <param name="deviceTomlFileContent">Content of the device toml file.</param>
        /// <returns></returns>
        private IEnumerable<ModuleReadModel> GetListOfModules(string deviceTomlFileContent)
        {
            var listOfModules = new List<ModuleReadModel>();

            if (!string.IsNullOrWhiteSpace(deviceTomlFileContent))
            {
                var data = GetTomlData(deviceTomlFileContent);

                listOfModules = data.Module;
            }

            // fix the indexes.
            listOfModules = listOfModules.Select((module, index) => new ModuleReadModel { Id = index, Config = module.Config, Name = module.Name, UUID = module.UUID }).ToList();

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
    }
}
