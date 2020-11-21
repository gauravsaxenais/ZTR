namespace ZTR.Framework.Security
{
    using System;
    using EnsureThat;
    using FluentValidation;
    using ZTR.Framework.Business;

    public static class RuleBuilderExtensions
    {
        public static IRuleBuilderOptions<TModel, string> PasswordValidation<TModel>(this IRuleBuilder<TModel, string> ruleBuilder, PasswordPolicyOptions passwordPolicyOptions, Enum passwordEmpty, Enum passwordTooShort)
        where TModel : class
        {
            EnsureArg.IsNotNull(passwordPolicyOptions, nameof(passwordPolicyOptions));

            return ruleBuilder
                .NotEmpty().WithErrorEnum(passwordEmpty)
                .MinimumLength(passwordPolicyOptions.MinimumLength).WithErrorEnum(passwordTooShort);
        }
    }
}
