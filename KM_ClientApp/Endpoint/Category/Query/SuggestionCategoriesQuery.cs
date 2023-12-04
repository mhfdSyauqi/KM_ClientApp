using KM_ClientApp.Commons.Mediator;
using KM_ClientApp.Commons.Shared;
using KM_ClientApp.Models.Request;
using KM_ClientApp.Models.Response;

namespace KM_ClientApp.Endpoint.Category.Query;

public record SuggestionCategoriesQuery(SuggestionCategoriesRequest Request) : IQuery<SuggestionCategoriesResponse>;

public class SuggestionCategoriesQueryHandler : IQueryHandler<SuggestionCategoriesQuery, SuggestionCategoriesResponse>
{
    private readonly ICategoryRepository _categoryRepository;

    public SuggestionCategoriesQueryHandler(ICategoryRepository categoryRepository)
    {
        _categoryRepository = categoryRepository;
    }

    public async Task<Result<SuggestionCategoriesResponse>> Handle(SuggestionCategoriesQuery request, CancellationToken cancellationToken)
    {
        var suggestion = await _categoryRepository.GetSuggestionCategoryAsync(request.Request, cancellationToken);

        var response = suggestion.Select(row => new SuggestionCategoriesResponse()
        {
            Items = suggestion.Select(item => new ItemCategoryResponse() { Id = item.Uid.ToString("N"), Name = item.Name }).ToList(),
            Paginations = new()
            {
                Current = row.Current,
                Next = row.Next,
                Previous = row.Previous,
            }
        }).FirstOrDefault();

        if (suggestion == null || response == null)
        {
            return Result.Failure<SuggestionCategoriesResponse>(new(
                "Category.NotFound",
                "There is no category found in database, please check your input"
                ));
        }

        return Result.Success(response);
    }
}
