namespace KM_ClientApp.Models.Response;

public class PageCategoryResponse
{
    public int Current { get; set; } = 1;
    public int? Next { get; set; }
    public int? Previous { get; set; }
}
