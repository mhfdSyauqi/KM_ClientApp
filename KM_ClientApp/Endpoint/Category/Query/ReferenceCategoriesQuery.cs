using FluentValidation;
using KM_ClientApp.Commons.Entity;
using KM_ClientApp.Commons.Mediator;
using KM_ClientApp.Commons.Shared;
using KM_ClientApp.Models.Request;
using KM_ClientApp.Models.Response;

namespace KM_ClientApp.Endpoint.Category.Query;

public record ReferenceCategoriesQuery(ReferenceCategoriesRequest Request) : IQuery<ReferenceCategoriesResponse>;

public class ReferenceCategoriesQueryHandler : IQueryHandler<ReferenceCategoriesQuery, ReferenceCategoriesResponse>
{
    private readonly ICategoryRepository _categoryRepository;
    private readonly IValidator<ReferenceCategoriesQuery> _validator;

    public ReferenceCategoriesQueryHandler(ICategoryRepository categoryRepository, IValidator<ReferenceCategoriesQuery> validator)
    {
        _categoryRepository = categoryRepository;
        _validator = validator;
    }


    public async Task<Result<ReferenceCategoriesResponse>> Handle(ReferenceCategoriesQuery request, CancellationToken cancellationToken)
    {
        await _validator.ValidateAndThrowAsync(request, cancellationToken);

        var reference = await _categoryRepository.GetReferenceCategoryAsync(request.Request, cancellationToken);

        var response = new ReferenceCategoriesResponse()
        {
            Ref_Id = reference.HasValue ? reference.Value.ToString("N") : null
        };

        return Result.Success(response);
    }
}

public class ReferenceCategoriesQueryValidator : AbstractValidator<ReferenceCategoriesQuery>
{
    public ReferenceCategoriesQueryValidator()
    {
        RuleFor(key => key.Request.Id).BeValidGuid();
    }
}
