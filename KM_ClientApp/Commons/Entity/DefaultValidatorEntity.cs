using FluentValidation;

namespace KM_ClientApp.Commons.Entity;

public static class DefaultValidatorEntity
{
    public static IRuleBuilderOptions<T, string> BeValidGuid<T>(this IRuleBuilder<T, string> ruleBuilder)
    {
        return ruleBuilder
                .NotNull()
                .NotEmpty()
                .Must(prop =>
                {
                    bool isValidGuid = Guid.TryParse(prop, out Guid result);
                    return isValidGuid;
                })
                .WithMessage("Identity is not vaild");
    }

    public static IRuleBuilderOptions<T, string?> BeValidGuidOptional<T>(this IRuleBuilder<T, string?> ruleBuilder)
    {
        return ruleBuilder
                .NotNull()
                .NotEmpty()
                .Must(prop =>
                {
                    bool isValidGuid = Guid.TryParse(prop, out Guid result);
                    return isValidGuid;
                })
                .WithMessage("Identity is not vaild");
    }

    public static IRuleBuilderOptions<T, string> BeValidUserName<T>(this IRuleBuilder<T, string> ruleBuilder)
    {
        return ruleBuilder
                .NotNull()
                .NotEmpty()
                .Must(prop =>
                {
                    if (prop != null && prop != "NotAuthUser")
                    {
                        return true;
                    }

                    return false;
                })
                .WithMessage("Unauthorize user");
    }
}
