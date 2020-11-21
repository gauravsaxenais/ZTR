namespace ZTR.Framework.Test.Fakes
{
    using System.Globalization;
    using ZTR.Framework.Business;

    public abstract class ModelWithCodeFaker<TModelWithCode> : ModelFaker<TModelWithCode>
        where TModelWithCode : class, IModelWithCode
    {
        public ModelWithCodeFaker()
        {
            RuleFor(x => x.Code, value => $"CODE_{value.Random.Guid().ToString("N", CultureInfo.InvariantCulture)}");
        }
    }
}
