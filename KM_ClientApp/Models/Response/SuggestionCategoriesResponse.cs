namespace KM_ClientApp.Models.Response;

public class SuggestionCategoriesResponse
{
    public List<ItemCategoryResponse> Items { get; set; } = new();
    public PageCategoryResponse Paginations { get; set; } = new();
}
