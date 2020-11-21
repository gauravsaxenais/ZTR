namespace ZTR.Framework.Business.Test.FixtureSetup.Fakes
{
    using System;
    using ZTR.Framework.Business.Test.FixtureSetup.Business.Widget.Models;
    using ZTR.Framework.Test.Fakes;

    public class WidgetFaker<TModel> : ModelWithCodeNameDescriptionFaker<TModel>
        where TModel : WidgetCreateModel
    {
        public WidgetFaker()
        {
            RuleFor(x => x.SomeDateTimePropertyPreciseToTheMinute, () => new DateTimeOffset(2019, 4, 16, 14, 21, 0, 0, TimeSpan.Zero));
            RuleFor(x => x.SomeDateTimePropertyPreciseToTheSecond, () => new DateTimeOffset(3000, 4, 16, 14, 21, 0, 0, TimeSpan.Zero));
            RuleFor(x => x.SomeNullableDateTimePropertyPreciseToTheMinute, () => new DateTimeOffset(3000, 4, 16, 14, 21, 0, 0, TimeSpan.Zero));
            RuleFor(x => x.SomeNullableDateTimePropertyPreciseToTheSecond, () => new DateTimeOffset(3000, 4, 16, 14, 21, 0, 0, TimeSpan.Zero));
            RuleFor(x => x.SomeDateTimePropertyPreciseToTheHour, () => new DateTimeOffset(3001, 2, 4, 3, 0, 0, TimeSpan.Zero));
            RuleFor(x => x.SomeNullableDateTimePropertyPreciseToTheHour, () => new DateTimeOffset(3001, 2, 4, 3, 0, 0, TimeSpan.Zero));
            RuleFor(x => x.SomeDateTimePropertyPreciseToTheDay, () => new DateTimeOffset(3001, 2, 4, 0, 0, 0, TimeSpan.Zero));
            RuleFor(x => x.SomeNullableDateTimePropertyPreciseToTheDay, () => new DateTimeOffset(3001, 2, 4, 0, 0, 0, TimeSpan.Zero));
        }
    }
}
