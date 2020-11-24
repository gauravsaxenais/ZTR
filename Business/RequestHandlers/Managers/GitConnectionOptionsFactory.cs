namespace Business.RequestHandlers.Managers
{
    using Business.Configuration;
    using Business.RequestHandlers.Interfaces;
    using System;
    using ZTR.Framework.Business.File;

    public class GitConnectionOptionsFactory : IGitConnectionOptionsFactory
    {
        public GitConnectionOptions GetGitConnectionOption(GitConnectionOptionType connectionType)
        {
            switch (connectionType)
            {
                case GitConnectionOptionType.Module:
                    return new ModuleGitConnectionOptions();
                case GitConnectionOptionType.Device:
                    return new DeviceGitConnectionOptions();
                case GitConnectionOptionType.Block:
                    return new BlockGitConnectionOptions();
                default:
                    throw new NotImplementedException();
            }
        }
    }
}
