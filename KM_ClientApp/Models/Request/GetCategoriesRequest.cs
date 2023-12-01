namespace KM_ClientApp.Models.Request;

public class GetCategoriesRequest
{
    public string? Searched_Identity { get; set; }
    public int Current_Page { get; set; } = 1;
}
