namespace Business.Configuration
{
    using System.IO;
    using ZTR.Framework.Business.File;

    public sealed class DeviceGitConnectionOptions : GitConnectionOptions, IGitConnectionOptions
    {
        public DeviceGitConnectionOptions()
        {
        }

        // username and password is empty as view and clone
        // options doesnot require login on ZTR network.
        public DeviceGitConnectionOptions(string gitLocalFolder, string userName, string password, string gitRepositoryUrl, TomlConfigurationFile tomlConfiguration) :
            base(gitLocalFolder, userName, password, gitRepositoryUrl)
        {
            TomlConfiguration = tomlConfiguration;
        }

        public TomlConfigurationFile TomlConfiguration { get; set; }

        public void SetConnection()
        {
            var currentDirectory = Path.GetDirectoryName(System.Reflection.Assembly.GetExecutingAssembly().Location);

            this.GitLocalFolder = Path.Combine(currentDirectory, this.GitLocalFolder);
            this.TomlConfiguration.DeviceFolder = Path.Combine(this.GitLocalFolder, this.TomlConfiguration.DeviceFolder);
        }

        public override string ToString()
        {
            return $"DeviceGitConnectionOptions(${this.GitLocalFolder} {this.GitRepositoryUrl} {this.TomlConfiguration})";
        }
    }
}
