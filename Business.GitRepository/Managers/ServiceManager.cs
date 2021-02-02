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
        private readonly ILogger<ServiceManager> _logger;

        protected string AppPath { get; }
        protected IGitRepositoryManager RepoManager { get; set; }
        protected IGitConnectionOptions ConnectionOptions { get; set; }

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
            ConnectionOptions = gitConnectionOptions;
            RepoManager = repoManager;

            AppPath = GetCurrentAppPath();
            SetConnection(ConnectionOptions);
        }

        /// <summary>
        /// Sets the connection.
        /// </summary>
        /// <param name="connectionOptions">The connection options.</param>
        /// <exception cref="CustomArgumentException">Current directory path is not valid.</exception>
        public void SetConnection(IGitConnectionOptions connectionOptions)
        {
            _logger.LogInformation("Setting git repository connection");
            
            ConnectionOptions.GitLocalFolder = Path.Combine(AppPath, connectionOptions.GitLocalFolder);

            SetupDependencies(ConnectionOptions);
            RepoManager.SetConnectionOptions(ConnectionOptions);
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
