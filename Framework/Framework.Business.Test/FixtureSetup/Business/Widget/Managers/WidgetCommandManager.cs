namespace ZTR.Framework.Business.Test.FixtureSetup.Business.Widget.Managers
{
    using ZTR.Framework.Business;
    using ZTR.Framework.Business.Test.FixtureSetup.Business.Widget.Models;
    using ZTR.Framework.Business.Test.FixtureSetup.Business.Widget.Validators;
    using ZTR.Framework.Business.Test.FixtureSetup.DataAccess;
    using ZTR.Framework.Business.Test.FixtureSetup.DataAccess.Entities;
    using AutoMapper;
    using Microsoft.Extensions.Logging;

    public class WidgetCommandManager
        : CodeCommandManager<TestDbContext, WidgetErrorCode, Widget, WidgetCreateModel, WidgetUpdateModel>
    {
        public WidgetCommandManager(
            TestDbContext databaseContext,
            WidgetCreateModelValidator createModelValidator,
            WidgetUpdateModelValidator updateModelValidator,
            ILogger<WidgetCommandManager> logger,
            IMapper mapper)
            : base(databaseContext, createModelValidator, updateModelValidator, logger, mapper, WidgetErrorCode.IdDoesNotExist, WidgetErrorCode.CodeNotUnique, WidgetErrorCode.IdNotUnique)
        {
        }
    }
}
