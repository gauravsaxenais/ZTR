namespace ZTR.Framework.Business
{
    using FluentValidation;

    public class ModelValidator<TModel> : AbstractValidator<TModel>
        where TModel : IModel
    {
        public ModelValidator()
        {
        }
    }
}
