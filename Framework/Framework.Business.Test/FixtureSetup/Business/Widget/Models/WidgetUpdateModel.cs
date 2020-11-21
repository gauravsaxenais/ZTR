namespace ZTR.Framework.Business.Test.FixtureSetup.Business.Widget.Models
{
    using ZTR.Framework.Business;

    public class WidgetUpdateModel : WidgetCreateModel, IModelWithId
    {
        public long Id { get; set; }
    }
}
