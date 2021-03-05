namespace ZTR.M7Config.Business.GitRepository.Interfaces
{
    using ZTR.Framework.Configuration;

    /// <summary>
    /// Base class for Service ZTR.M7Config.Business..
    /// </summary>
    /// <typeparam name="T"></typeparam>
    /// <seealso cref="IManager" />
    public interface IServiceManager
    {
        void SetConnection(GitConnectionOptions connectionOptions);
    }
}
