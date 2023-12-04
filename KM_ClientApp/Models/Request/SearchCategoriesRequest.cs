namespace KM_ClientApp.Models.Request;

public class SearchCategoriesRequest
{
    public string Searched_Keyword { get; set; } = string.Empty;
    public int Current_Page { get; set; } = 1;
}
