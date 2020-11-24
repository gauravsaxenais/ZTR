namespace Business.RequestHandlers.Interfaces
{
    using Business.Configuration;
    using ZTR.Framework.Business.File;

    public interface IGitConnectionOptionsFactory
    {
        GitConnectionOptions GetGitConnectionOption(GitConnectionOptionType connectionType);
    }
}
