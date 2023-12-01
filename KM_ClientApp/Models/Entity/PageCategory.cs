namespace KM_ClientApp.Models.Entity;

public class PageCategory
{
    public int Current { get; set; } = 1;
    public int? Next { get; set; }
    public int? Previous { get; set; }
}
