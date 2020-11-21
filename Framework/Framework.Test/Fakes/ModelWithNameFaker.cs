namespace ZTR.Framework.Test.Fakes
{
    using ZTR.Framework.Business;

    public abstract class ModelWithNameFaker<TModelWithName> : ModelFaker<TModelWithName>
        where TModelWithName : class, IModelWithName
    {
        public ModelWithNameFaker()
        {
            RuleFor(x => x.Name, value => value.Hacker.Noun());
        }
    }
}
