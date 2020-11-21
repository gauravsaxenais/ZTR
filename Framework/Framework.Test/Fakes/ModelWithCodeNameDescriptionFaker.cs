namespace ZTR.Framework.Test.Fakes
{
    using ZTR.Framework.Business;

    public abstract class ModelWithCodeNameDescriptionFaker<TModelWithCodeNameDescription> : ModelWithCodeNameFaker<TModelWithCodeNameDescription>
        where TModelWithCodeNameDescription : class, IModelWithCode, IModelWithName, IModelWithDescription
    {
        public ModelWithCodeNameDescriptionFaker()
        {
            RuleFor(x => x.Description, value => value.Hacker.Noun());
        }
    }
}
