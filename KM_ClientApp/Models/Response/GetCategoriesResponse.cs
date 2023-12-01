namespace KM_ClientApp.Models.Response;

public class GetCategoriesResponse
{
    public string? Searched_Identity { get; set; }
    public int Layer { get; set; }
    public List<ItemCategoryResponse> Items { get; set; } = new();
    public PageCategoryResponse Paginations { get; set; } = new();
}
