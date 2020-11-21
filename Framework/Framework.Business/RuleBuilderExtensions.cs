namespace ZTR.Framework.Business
{
    using System;
    using ZTR.Framework.DataAccess;
    using FluentValidation;

    public static class RuleBuilderExtensions
    {
        public static IRuleBuilderOptions<TModel, long> IdValidation<TModel>(this IRuleBuilder<TModel, long> ruleBuilder, Enum idMustBeGreaterThanZero)
            where TModel : class, IModelWithId
        {
            return ruleBuilder
                .GreaterThan(0).WithErrorEnum(idMustBeGreaterThanZero);
        }

        public static IRuleBuilderOptions<TModel, string> UrlValidation<TModel>(this IRuleBuilder<TModel, string> ruleBuilder, Enum urlNotWellFormed, UriKind uriKind = UriKind.Absolute)
            where TModel : class
        {
            return ruleBuilder
                .Must(x => Uri.IsWellFormedUriString(x, uriKind)).WithMessage("The format of '{PropertyName}' must be a well formed URL.").WithErrorEnum(urlNotWellFormed);
        }

        public static IRuleBuilderOptions<TModel, string> NameValidation<TModel>(this IRuleBuilder<TModel, string> ruleBuilder, Enum nameRequired, Enum nameTooLong, int maximumLength = BaseConstants.DataLengths.Name)
            where TModel : class, IModelWithName
        {
            return ruleBuilder
                .NotEmpty().WithErrorEnum(nameRequired)
                .MaximumLength(maximumLength).WithErrorEnum(nameTooLong);
        }

        public static IRuleBuilderOptions<TModel, string> DescriptionValidation<TModel>(this IRuleBuilder<TModel, string> ruleBuilder, Enum descriptionRequired, Enum descriptionTooLong, int maximumLength = BaseConstants.DataLengths.Description)
            where TModel : class, IModelWithDescription
        {
            return ruleBuilder
                .NotEmpty().WithErrorEnum(descriptionRequired)
                .MaximumLength(maximumLength).WithErrorEnum(descriptionTooLong);
        }

        public static IRuleBuilderOptions<TModel, string> DescriptionValidation<TModel>(this IRuleBuilder<TModel, string> ruleBuilder, Enum descriptionTooLong, int maximumLength = BaseConstants.DataLengths.Description)
            where TModel : class, IModelWithDescription
        {
            return ruleBuilder
                .MaximumLength(maximumLength).WithErrorEnum(descriptionTooLong);
        }

        public static IRuleBuilderOptions<TModel, string> CodeValidation<TModel>(this IRuleBuilder<TModel, string> ruleBuilder, Enum codeRequired, Enum codeTooLong, int maximumLength = BaseConstants.DataLengths.Code)
            where TModel : class, IModelWithCode
        {
            return ruleBuilder
                .NotEmpty().WithErrorEnum(codeRequired)
                .MaximumLength(maximumLength).WithErrorEnum(codeTooLong);
        }
    }
}
