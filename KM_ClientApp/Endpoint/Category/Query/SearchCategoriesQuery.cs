using FluentValidation;
using KM_ClientApp.Commons.Mediator;
using KM_ClientApp.Commons.Shared;
using KM_ClientApp.Models.Request;
using KM_ClientApp.Models.Response;

namespace KM_ClientApp.Endpoint.Category.Query;

public record SearchCategoriesQuery(SearchCategoriesRequest Request) : IQuery<SearchCategoriesResponse>;

public class SearchCategoriesQueryHandler : IQueryHandler<SearchCategoriesQuery, SearchCategoriesResponse>
{
    private readonly ICategoryRepository _categoryRepository;
    private readonly IValidator<SearchCategoriesQuery> _validator;

    public SearchCategoriesQueryHandler(ICategoryRepository categoryRepository, IValidator<SearchCategoriesQuery> validator)
    {
        _categoryRepository = categoryRepository;
        _validator = validator;
    }

    public async Task<Result<SearchCategoriesResponse>> Handle(SearchCategoriesQuery request, CancellationToken cancellationToken)
    {
        await _validator.ValidateAndThrowAsync(request, cancellationToken);

        var searched = await _categoryRepository.SearchCategoryByKeywordAsync(request.Request, cancellationToken);

        var response = searched.Select(row => new SearchCategoriesResponse()
        {
            Searched_Keyword = request.Request.Searched_Keyword,
            Items = searched.Select(item => new ItemCategoryResponse { Id = item.Uid.ToString("N"), Name = item.Name, Has_Content = item.Has_Content }).ToList(),
            Paginations = new()
            {
                Current = row.Current,
                Next = row.Next,
                Previous = row.Previous,
            }
        }).FirstOrDefault();

        if (searched == null || response == null)
        {
            return Result.Failure<SearchCategoriesResponse>(new(
                "Category.NotFound",
                "There is no category found in database, please check your input"
                ));
        }

        return Result.Success(response);

    }
}

public class SearchCategoriesQueryValidator : AbstractValidator<SearchCategoriesQuery>
{
    public SearchCategoriesQueryValidator()
    {
        RuleFor(key => key.Request.Searched_Keyword).NotEmpty().NotNull().MinimumLength(3);
    }
}


