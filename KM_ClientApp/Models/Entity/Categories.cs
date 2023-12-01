namespace KM_ClientApp.Models.Entity;

public class Categories
{
    public Guid Uid { get; set; }
    public string Name { get; set; } = string.Empty;
    public Guid? Uid_Reference { get; set; }
    public int Layer { get; set; }
    public int Current { get; set; }
    public int? Next { get; set; }
    public int? Previous { get; set; }
}
