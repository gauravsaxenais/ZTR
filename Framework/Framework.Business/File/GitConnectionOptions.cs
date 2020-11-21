namespace ZTR.Framework.Business.File
{
    using ZTR.Framework.Configuration;

    public class GitConnectionOptions : ConfigurationOptions
    {
        public GitConnectionOptions()
        {
        }
        public GitConnectionOptions(string gitLocalFolder, string userName, string password, string gitRepositoryUrl)
        {
            GitLocalFolder = gitLocalFolder;
            UserName = userName;
            Password = password;
            GitRepositoryUrl = gitRepositoryUrl;
        }
        public string GitLocalFolder { get; set; }
        public string UserName { get; set; }
        public string Password { get; set; }
        public string GitRepositoryUrl { get; set; }

        public override string ToString()
        {
            return $"GitConnectionOptions(${this.GitLocalFolder} {this.UserName} {this.Password} {this.GitRepositoryUrl})";
        }
    }
}
