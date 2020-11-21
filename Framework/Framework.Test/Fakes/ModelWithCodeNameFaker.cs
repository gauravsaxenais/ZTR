namespace ZTR.Framework.Test.Fakes
{
    using ZTR.Framework.Business;

    public abstract class ModelWithCodeNameFaker<TModelWithCodeName> : ModelWithCodeFaker<TModelWithCodeName>
        where TModelWithCodeName : class, IModelWithCode, IModelWithName
    {
        public ModelWithCodeNameFaker()
        {
            RuleFor(x => x.Name, value => value.Hacker.Noun());
        }
    }
}
