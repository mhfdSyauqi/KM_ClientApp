namespace KM_ClientApp.Models.Entity;

public class GetCategories
{
    public Guid? Searched_Identity { get; set; }
    public int Layer { get; set; }
    public List<ItemCategory> Items { get; set; } = new();
    public PageCategory Paginations { get; set; } = new();
}
