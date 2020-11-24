namespace Business.Configuration
{
    using ZTR.Framework.Business.File;
    public sealed class ModuleGitConnectionOptions : GitConnectionOptions, IGitConnectionOptions
    {
        public ModuleGitConnectionOptions()
        {
        }

        // username and password is empty as view and clone
        // options doesnot require login on ZTR network.
        public ModuleGitConnectionOptions(string gitLocalFolder, string userName, string password, string gitRepositoryUrl, TomlConfigurationFile tomlConfiguration) :
            base(gitLocalFolder, userName, password, gitRepositoryUrl)
        {
        }

        public void SetConnection()
        {
            throw new System.NotImplementedException();
        }

        public override string ToString()
        {
            return $"ModuleGitConnectionOptions(${this.GitLocalFolder} {this.GitRepositoryUrl})";
        }
    }
}
