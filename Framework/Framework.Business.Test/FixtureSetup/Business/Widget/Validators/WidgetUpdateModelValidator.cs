namespace ZTR.Framework.Business.Test.FixtureSetup.Business.Widget.Validators
{
    using ZTR.Framework.Business;
    using ZTR.Framework.Business.Test.FixtureSetup.Business.Widget.Models;

    public class WidgetUpdateModelValidator : ModelValidator<WidgetUpdateModel>
    {
        public WidgetUpdateModelValidator()
        {
            RuleFor(x => x.Name)
                .NameValidation(WidgetErrorCode.NameRequired, WidgetErrorCode.NameTooLong);

            RuleFor(x => x.Code)
                .CodeValidation(WidgetErrorCode.CodeRequired, WidgetErrorCode.CodeTooLong);

            RuleFor(x => x.Description)
                .DescriptionValidation(WidgetErrorCode.DescriptionRequired, WidgetErrorCode.DescriptionTooLong);
        }
    }
}
