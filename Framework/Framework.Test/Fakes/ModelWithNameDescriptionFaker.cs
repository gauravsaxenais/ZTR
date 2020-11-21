namespace ZTR.Framework.Test.Fakes
{
    using ZTR.Framework.Business;

    public abstract class ModelWithNameDescriptionFaker<TModelWithNameDescription> : ModelWithNameFaker<TModelWithNameDescription>
        where TModelWithNameDescription : class, IModelWithName, IModelWithDescription
    {
        public ModelWithNameDescriptionFaker()
        {
            RuleFor(x => x.Description, value => value.Hacker.Phrase());
        }
    }
}
