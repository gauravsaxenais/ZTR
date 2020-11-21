namespace ZTR.Framework.Business.Test.FixtureSetup.ModelValidatorTests
{
    using ZTR.Framework.Business;

    public class PersonModel : IModel
    {
        // Its for a unit test.
        public string ProfileUrl { get; set; }

        public string PortalUrl { get; set; }
    }
}
