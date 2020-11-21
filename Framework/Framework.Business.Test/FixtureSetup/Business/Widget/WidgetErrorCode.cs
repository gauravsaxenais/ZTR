namespace ZTR.Framework.Business.Test.FixtureSetup.Business.Widget
{
    public enum WidgetErrorCode
    {
        Unknown,
        IdMustBeGreaterThanZero,
        IdDoesNotExist,
        IdNotUnique,
        NameRequired,
        NameTooLong,
        CodeRequired,
        CodeTooLong,
        CodeNotUnique,
        DescriptionRequired,
        DescriptionTooLong,
        CategoryNotFound,
        NotPreciseToSecond,
        NotPreciseToSecondNullable,
        NotPreciseToMinute,
        NotPreciseToMinuteNullable,
        NotPreciseToHours,
        NotPreciseToHoursNullable,
        NotPreciseToDays,
        NotPreciseToDaysNullable
    }
}
