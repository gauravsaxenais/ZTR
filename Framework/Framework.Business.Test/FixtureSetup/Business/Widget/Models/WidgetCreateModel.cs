namespace ZTR.Framework.Business.Test.FixtureSetup.Business.Widget.Models
{
    using System;
    using ZTR.Framework.Business.Models;

    public class WidgetCreateModel : ModelWithCodeNameDescription
    {
        public string SomeText { get; set; }

        public long ForeignKeyId { get; set; }

        public DateTimeOffset SomeDateTimePropertyPreciseToTheMinute { get; set; }

        public DateTimeOffset SomeDateTimePropertyPreciseToTheSecond { get; set; }

        public DateTimeOffset? SomeNullableDateTimePropertyPreciseToTheMinute { get; set; }

        public DateTimeOffset? SomeNullableDateTimePropertyPreciseToTheSecond { get; set; }

        public DateTimeOffset SomeDateTimePropertyPreciseToTheHour { get; set; }

        public DateTimeOffset? SomeNullableDateTimePropertyPreciseToTheHour { get; set; }

        public DateTimeOffset SomeDateTimePropertyPreciseToTheDay { get; set; }

        public DateTimeOffset? SomeNullableDateTimePropertyPreciseToTheDay { get; set; }
    }
}
