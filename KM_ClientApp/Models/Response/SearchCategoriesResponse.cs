namespace KM_ClientApp.Models.Response;

public class SearchCategoriesResponse
{
    public string Searched_Keyword { get; set; } = string.Empty;
    public List<ItemCategoryResponse> Items { get; set; } = new();
    public PageCategoryResponse Paginations { get; set; } = new();
}
