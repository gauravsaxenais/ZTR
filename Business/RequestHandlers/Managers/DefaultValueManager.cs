namespace Business.RequestHandlers.Managers
{
    using Business.Configuration;
    using Business.Models.ConfigModels;
    using Business.Parsers;
    using Business.RequestHandlers.Interfaces;
    using EnsureThat;
    using Microsoft.Extensions.Logging;
    using Nett;
    using System;
    using System.Collections.Generic;
    using System.Diagnostics;
    using System.IO;
    using System.Linq;
    using System.Threading.Tasks;
    using ZTR.Framework.Business;
    using ZTR.Framework.Business.File.FileReaders;

    public class DefaultValueManager : Manager, IDefaultValueManager
    {
        private readonly IGitRepositoryManager _gitRepoManager;
        private readonly DeviceGitConnectionOptions _deviceGitConnectionOptions;

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

        public async Task GetDefaultValuesAllModulesAsync(string firmwareVersion, string deviceType)
        {
            await GetDataFromDefaultFileAsync(firmwareVersion, deviceType);

            //return listOfModules;
        }

        private async Task GetDataFromDefaultFileAsync(string firmwareVersion, string deviceType)
        {
            var inputFileLoader = new InputFileLoader();

            var compilerPath = string.Empty;
            inputFileLoader.GetProtocPath(out compilerPath);

            if(!string.IsNullOrWhiteSpace(compilerPath))
            {
                inputFileLoader.GenerateFiles("power.proto");
            }

            var fileContent = await GetDefaultData(firmwareVersion, deviceType);


            //var listOfMessages = 
            if (!string.IsNullOrWhiteSpace(fileContent))
            {
                var defaultFileData = GetTomlData(fileContent);

                //listOfModules = defaultFileData.Module;
            }

            //return listOfModules;
        }

        public static List<T> ReadDataModel<T>(string data, string fieldToRead, TomlSettings settings) where T : class, new()
        {
            EnsureArg.IsNotEmptyOrWhiteSpace(data, nameof(data));
            EnsureArg.IsNotNull(settings, nameof(settings));
            EnsureArg.IsNotEmptyOrWhiteSpace(fieldToRead, nameof(fieldToRead));

            TomlTable fileData = null;

            fileData = Toml.ReadString(data, settings);

            var readModels = (TomlTable)fileData[fieldToRead];

            var items = new List<T>();
            var dictionary = readModels.Rows.ToDictionary(t => t.Key, t => (object)t.Value.ToString());

            items.Add(DictionaryExtensions.ToObject<T>(dictionary));
            return items;
        }

        private ConfigurationReadModel GetTomlData(string fileContent)
        {
            var tomlSettings = TomlFileReader.LoadLowerCaseTomlSettingsWithMappingForDefaultValues();

            var tomlData = TomlFileReader.ReadDataFromString<ConfigurationReadModel>(data: fileContent, settings: tomlSettings);

            return tomlData;
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

        private void GenerateCSharpFileFromProtoFile(string protoFileLocation, string csharpFileDirectory, string protoFileName)
        {
            // Use ProcessStartInfo class
            var psi = new ProcessStartInfo
            {
                FileName = @"C:\Users\gaurav.saxena\source\repos\ProtoAppGoogleProtoBuff\ProtoCompiler\protoc-3.13.0-win64\bin\protoc.exe",
                WindowStyle = ProcessWindowStyle.Hidden,
                UseShellExecute = false,
                CreateNoWindow = true,
                Arguments = $" --include_imports --include_source_info --proto_path={protoFileLocation} --csharp_out={csharpFileDirectory} {protoFileName}"
            };

            try
            {
                // Start the process with the info we specified.
                // Call WaitForExit and then the using statement will close.
                using (var exeProcess = Process.Start(psi))
                {
                    exeProcess.WaitForExit();
                }
            }
            catch (Exception)
            {
            }
        }
    }
}
