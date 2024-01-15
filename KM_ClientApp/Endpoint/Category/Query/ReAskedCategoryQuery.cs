using FluentValidation;
using KM_ClientApp.Commons.Entity;
using KM_ClientApp.Commons.Mediator;
using KM_ClientApp.Commons.Shared;
using KM_ClientApp.Models.Request;
using KM_ClientApp.Models.Response;

namespace KM_ClientApp.Endpoint.Category.Query;

public record ReAskedCategoryQuery(ReAskedRequest Request) : IQuery<ReAskedResponse>;

public class ReAskedCategoryQueryHandler : IQueryHandler<ReAskedCategoryQuery, ReAskedResponse>
{
    private readonly ICategoryRepository _categoryRepository;
    private readonly IValidator<ReAskedCategoryQuery> _validator;

    public ReAskedCategoryQueryHandler(IValidator<ReAskedCategoryQuery> validator, ICategoryRepository categoryRepository)
    {
        _validator = validator;
        _categoryRepository = categoryRepository;
    }

    public async Task<Result<ReAskedResponse>> Handle(ReAskedCategoryQuery request, CancellationToken cancellationToken)
    {
        await _validator.ValidateAndThrowAsync(request, cancellationToken);

        var reaskCount = await _categoryRepository.GetReAskedCategoryAsync(request.Request, cancellationToken);

        var response = new ReAskedResponse()
        {
            Count = reaskCount
        };

        return Result.Success(response);
    }
}


public class ReAskedCategoryQueryValidator : AbstractValidator<ReAskedCategoryQuery>
{
    public ReAskedCategoryQueryValidator()
    {
        RuleFor(key => key.Request.Create_By).BeValidUserName();
        RuleFor(key => key.Request.Category_Id).BeValidGuid();
    }
}