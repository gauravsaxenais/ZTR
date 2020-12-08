namespace Business.RequestHandlers.Managers
{
    using Business.Configuration;
    using Business.Core;
    using Business.RequestHandlers.Interfaces;
    using EnsureThat;
    using Microsoft.Extensions.Logging;
    using Nett;
    using Newtonsoft.Json;
    using Newtonsoft.Json.Linq;
    using Service.Core;
    using System;
    using System.Collections.Generic;
    using System.IO;
    using System.Linq;
    using System.Text;
    using System.Threading.Tasks;
    using ZTR.Framework.Business;

    /// <summary>
    /// Parses a toml file and returns arguments for a block
    /// among others.
    /// </summary>
    /// <seealso cref="ZTR.Framework.Business.Manager" />
    /// <seealso cref="Business.RequestHandlers.Interfaces.IBlockManager" />
    public class BlockManagerV : Manager, IBlockManager
    {
       
        #region Private Variables
        private readonly IGitRepositoryManager _repoManager;
        private readonly BlockGitConnectionOptions _blockGitConnectionOptions;

        private static readonly string fileArguments = "arguments";
        private static readonly string fileModules = "module";
        private static readonly string fileBlocks = "blocks";
        #endregion

        #region Constructors        

        /// <summary>
        /// Initializes a new instance of the <see cref="BlockManager"/> class.
        /// </summary>
        /// <param name="logger">The logger.</param>
        /// <param name="gitRepoManager">The git repo manager.</param>
        /// <param name="blockGitConnectionOptions">The block git connection options.</param>
        public BlockManagerV(ILogger<ModuleManager> logger, IGitRepositoryManager gitRepoManager, BlockGitConnectionOptions blockGitConnectionOptions) : base(logger)
        {
            EnsureArg.IsNotNull(logger, nameof(logger));
            EnsureArg.IsNotNull(blockGitConnectionOptions, nameof(blockGitConnectionOptions));

            _repoManager = gitRepoManager;
            _blockGitConnectionOptions = blockGitConnectionOptions;

            var currentDirectory = Path.GetDirectoryName(System.Reflection.Assembly.GetExecutingAssembly().Location);

            _blockGitConnectionOptions.GitLocalFolder = Path.Combine(currentDirectory, _blockGitConnectionOptions.GitLocalFolder);

            _repoManager.SetConnectionOptions(_blockGitConnectionOptions);
        }
        #endregion

        #region Public Methods

        /// <summary>
        /// Parses the toml files asynchronous.
        /// </summary>
        /// <param name="firmwareVersion">The firmware version.</param>
        /// <param name="deviceType">Type of the device.</param>
        /// <param name="parserType">Type of the parser.</param>
        /// <returns></returns>
        public async Task<string> ParseTomlFilesAsync(string firmwareVersion, string deviceType, string parserType)
        {
            string[] files = Directory.GetFiles($"{Global.RootPath}/TOML");
          
            Func<TomlObject, object> ToDict = null;
            ToDict = (o) =>
            {
                try
                {
                    return o is TomlTable ? ((TomlTable)o).ToDictionary(t => t.Key, t => ToDict(t.Value))
                           : (o is TomlArray ? ((TomlArray)o).Items.Select(u => ToDict(u)).ToArray()
                                : ((TomlTableArray)o).Items.Select(u => ToDict(u)).ToArray());
                }
                catch
                {
                    return ((TomlString)o).Value;
                }
            };
            var data = files.Take(1).Select(o => { 
                 var fileContent = Toml.ReadString(File.ReadAllText(o));                 
                 var dictionary = fileContent.ToDictionary(t => t.Key, t => ToDict(t.Value));
                // var final = dictionary.Where(o => o.Key == "macros").ToDictionary(t => t.Key, t => t.Value);
                 dictionary.Add("ID", 1);
                return JsonConvert.SerializeObject(dictionary, Formatting.None);
            }).ToArray();

            var a = await Task.FromResult("["+ string.Join(',',data) + "]");
            return a;
            
        }
        #endregion

    }
}
