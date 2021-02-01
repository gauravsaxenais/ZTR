namespace Business.GitRepository.Managers
{
    using EnsureThat;
    using Interfaces;
    using Microsoft.Extensions.Logging;
    using System.IO;
    using ZTR.Framework.Business;
    using ZTR.Framework.Business.Models;
    using ZTR.Framework.Configuration;

    public class ServiceManager : Manager, IServiceManager
    {
        private readonly IGitConnectionOptions _connectionOptions;
        private readonly ILogger<ServiceManager> _logger;
        private readonly IGitRepositoryManager _repoManager;
        private readonly string _appPath;

        protected string AppPath
        {
            get { return _appPath; }
            set => GetCurrentAppPath();
        }

        /// <summary>
        /// Initializes a new instance of the <see cref="DeviceServiceManager"/> class.
        /// </summary>
        /// <param name="logger">The logger.</param>
        /// <param name="gitConnectionOptions">The device git connection options.</param>
        public ServiceManager(ILogger<ServiceManager> logger, IGitConnectionOptions gitConnectionOptions, IGitRepositoryManager repoManager) : base(logger)
        {
            EnsureArg.IsNotNull(logger, nameof(logger));
            EnsureArg.IsNotNull(gitConnectionOptions, nameof(gitConnectionOptions));
            EnsureArg.IsNotNull(repoManager, nameof(repoManager));

            _logger = logger;
            _connectionOptions = gitConnectionOptions;
            _repoManager = repoManager;

            _appPath = GetCurrentAppPath();
            SetConnection(_connectionOptions);
        }

        /// <summary>
        /// Sets the connection.
        /// </summary>
        /// <param name="connectionOptions">The connection options.</param>
        /// <exception cref="CustomArgumentException">Current directory path is not valid.</exception>
        public void SetConnection(IGitConnectionOptions connectionOptions)
        {
            _logger.LogInformation("Setting git repository connection");
            

            _connectionOptions.GitLocalFolder = Path.Combine(currentDirectory, connectionOptions.GitLocalFolder);

            SetupDependencies(_connectionOptions);
            _repoManager.SetConnectionOptions(_connectionOptions);
        }

        protected virtual void SetupDependencies(IGitConnectionOptions connectionOptions)
        {
        }

        private string GetCurrentAppPath()
        {
            var currentDirectory = Path.GetDirectoryName(System.Reflection.Assembly.GetExecutingAssembly().Location);

            if (currentDirectory == null)
            {
                throw new CustomArgumentException("Current directory path is not valid.");
            }

            return currentDirectory;
        }
    }
}
