namespace KM_ClientApp.Models.Entity;

public class HeatCategories
{
    public Guid Session_Id { get; set; }
    public string User_Name { get; set; } = string.Empty;
    public string Heat_Name { get; set; } = string.Empty;
    public Guid? Heat_Id { get; set; }
}
