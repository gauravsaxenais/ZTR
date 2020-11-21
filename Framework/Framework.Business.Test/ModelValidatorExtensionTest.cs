namespace ZTR.Framework.Business.Test
{
    using ZTR.Framework.Business.Test.FixtureSetup.ModelValidatorTests;
    using Xunit;

    public class ModelValidatorExtensionTest
    {
        [Fact]
        public void UrlValidationWithBadUrlShouldThrowUrlNotWellFormedError()
        {
            var validator = new PersonModelValidator();
            var person = new PersonModel
            {
                ProfileUrl = "not a valid url"
            };

            var results = validator.Validate(person);

            Assert.False(results.IsValid);
            Assert.Contains(results.Errors, x => x.ErrorCode == PersonErrorCode.MalformedProfileUrl.ToString());

            var expectedErrorMessage = $"The format of 'Profile Url' must be a well formed URL.";
            var actualErrorMessage = results.Errors[0].ErrorMessage;
            Assert.Equal(expectedErrorMessage, actualErrorMessage);
        }

        [Fact]
        public void UrlValidationWithBadRelativeUrlShouldThrowUrlWellFormedError()
        {
            var validator = new PersonModelValidator();
            var person = new PersonModel
            {
                PortalUrl = "bad relative url"
            };

            var results = validator.Validate(person);

            Assert.False(results.IsValid);
            Assert.Contains(results.Errors, x => x.ErrorCode == PersonErrorCode.MalformedPortalUrl.ToString());
        }

        [Fact]
        public void UrlValidationWithValidAbsoluteOrRelativeUrlShouldNotThrowUrlWellFormedError()
        {
            var validator = new PersonModelValidator();
            var person = new PersonModel
            {
                ProfileUrl = "https://www.google.com/",
                PortalUrl = "api/query/RenderReport/GetReportData?LeadsSource=$leads_source.code"
            };

            var results = validator.Validate(person);

            Assert.True(results.IsValid);
        }
    }
}
