namespace ZTR.Framework.Business
{
    using EnsureThat;
    using Microsoft.Extensions.Logging;

    public abstract class Manager : IManager
    {
        protected Manager(ILogger logger)
        {
            EnsureArg.IsNotNull(logger, nameof(logger));

            Logger = logger;
        }

        protected ILogger Logger { get; private set; }
    }
}
