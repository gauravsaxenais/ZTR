namespace ZTR.Framework.Business.Test.FixtureSetup.ModelValidatorTests
{
    using ZTR.Framework.Business;

    public class PersonModelValidator : ModelValidator<PersonModel>
    {
        public PersonModelValidator()
        {
            RuleFor(x => x.ProfileUrl).UrlValidation(PersonErrorCode.MalformedProfileUrl);

            RuleFor(x => x.PortalUrl).UrlValidation(PersonErrorCode.MalformedPortalUrl, System.UriKind.Relative);
        }
    }
}
