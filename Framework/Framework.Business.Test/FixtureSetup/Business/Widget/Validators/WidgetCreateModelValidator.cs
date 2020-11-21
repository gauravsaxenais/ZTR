namespace ZTR.Framework.Business.Test.FixtureSetup.Business.Widget.Validators
{
    using ZTR.Framework.Business;
    using ZTR.Framework.Business.Test.FixtureSetup.Business.Widget.Models;

    public class WidgetCreateModelValidator : ModelValidator<WidgetCreateModel>
    {
        public WidgetCreateModelValidator()
        {
            RuleFor(x => x.Name)
                .NameValidation(WidgetErrorCode.NameRequired, WidgetErrorCode.NameTooLong);

            RuleFor(x => x.Code)
                .CodeValidation(WidgetErrorCode.CodeRequired, WidgetErrorCode.CodeTooLong);

            RuleFor(x => x.Description)
                .DescriptionValidation(WidgetErrorCode.DescriptionRequired, WidgetErrorCode.DescriptionTooLong);

            RuleFor(x => x.SomeDateTimePropertyPreciseToTheMinute)
                .IsPreciseToMinutes(WidgetErrorCode.NotPreciseToMinute);

            RuleFor(x => x.SomeDateTimePropertyPreciseToTheSecond)
                .IsPreciseToSeconds(WidgetErrorCode.NotPreciseToSecond);

            RuleFor(x => x.SomeNullableDateTimePropertyPreciseToTheMinute)
                .IsPreciseToMinutes(WidgetErrorCode.NotPreciseToMinuteNullable);

            RuleFor(x => x.SomeNullableDateTimePropertyPreciseToTheSecond)
                .IsPreciseToSeconds(WidgetErrorCode.NotPreciseToSecondNullable);

            RuleFor(x => x.SomeDateTimePropertyPreciseToTheHour)
                .IsPreciseToHours(WidgetErrorCode.NotPreciseToHours);

            RuleFor(x => x.SomeNullableDateTimePropertyPreciseToTheHour)
                .IsPreciseToHours(WidgetErrorCode.NotPreciseToHoursNullable);

            RuleFor(x => x.SomeDateTimePropertyPreciseToTheDay)
                .IsPreciseToDay(WidgetErrorCode.NotPreciseToDays);

            RuleFor(x => x.SomeNullableDateTimePropertyPreciseToTheDay)
                .IsPreciseToDay(WidgetErrorCode.NotPreciseToDaysNullable);
        }
    }
}
