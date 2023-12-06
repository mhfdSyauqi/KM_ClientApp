using FluentValidation;
using KM_ClientApp.Commons.Entity;
using KM_ClientApp.Commons.Mediator;
using KM_ClientApp.Commons.Shared;
using KM_ClientApp.Models.Request;

namespace KM_ClientApp.Endpoint.Category.Command;

public record HeatCategoriesCommand(HeatCategoriesRequest Request) : ICommand;

public class HeatCategoriesCommandHandler : ICommandHandler<HeatCategoriesCommand>
{
    private readonly ICategoryRepository _categoryRepository;
    private readonly IValidator<HeatCategoriesCommand> _validator;

    public HeatCategoriesCommandHandler(ICategoryRepository categoryRepository, IValidator<HeatCategoriesCommand> validator)
    {
        _categoryRepository = categoryRepository;
        _validator = validator;
    }

    public async Task<Result> Handle(HeatCategoriesCommand request, CancellationToken cancellationToken)
    {
        await _validator.ValidateAndThrowAsync(request, cancellationToken);

        await _categoryRepository.AddHeatCategoryAsync(request.Request, cancellationToken);

        return Result.Success();
    }
}

public class HeatCategoriesCommandValidator : AbstractValidator<HeatCategoriesCommand>
{
    public HeatCategoriesCommandValidator()
    {
        RuleFor(key => key.Request.Session_Id).BeValidGuid();
        RuleFor(key => key.Request.User_Name).BeValidUserName();
        RuleFor(key => key.Request.Heat_Name).NotEmpty();

        When(props => !string.IsNullOrWhiteSpace(props.Request.Heat_Id), () =>
        {
            RuleFor(key => key.Request.Heat_Id).BeValidGuidOptional();
        });
    }
}
