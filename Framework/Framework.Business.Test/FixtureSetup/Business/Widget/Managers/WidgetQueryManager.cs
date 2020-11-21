namespace ZTR.Framework.Business.Test.FixtureSetup.Business.Widget.Managers
{
    using AutoMapper;
    using Microsoft.Extensions.Logging;
    using ZTR.Framework.Business;
    using ZTR.Framework.Business.Test.FixtureSetup.Business.Widget.Models;
    using ZTR.Framework.Business.Test.FixtureSetup.DataAccess;
    using ZTR.Framework.Business.Test.FixtureSetup.DataAccess.Entities;

    public class WidgetQueryManager
        : CodeQueryManager<TestDbContext, Widget, WidgetReadModel>
    {
        public WidgetQueryManager(
            TestDbContext databaseContext,
            ILogger<WidgetQueryManager> logger,
            IMapper mapper)
            : base(databaseContext, logger, mapper)
        {
        }
    }
}
