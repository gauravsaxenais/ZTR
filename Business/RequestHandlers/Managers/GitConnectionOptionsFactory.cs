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
                    return (ModuleGitConnectionOptions)Activator.CreateInstance(Type.GetType(typeof(ModuleGitConnectionOptions).AssemblyQualifiedName, false));
                case GitConnectionOptionType.Device:
                    return (DeviceGitConnectionOptions)Activator.CreateInstance(Type.GetType(typeof(DeviceGitConnectionOptions).AssemblyQualifiedName, false));
                case GitConnectionOptionType.Block:
                    return (BlockGitConnectionOptions)Activator.CreateInstance(Type.GetType(typeof(BlockGitConnectionOptions).AssemblyQualifiedName, false));
                default:
                    throw new NotImplementedException();
            }
        }
    }
}
