namespace KM_ClientApp.Models.Request;

public class HeatCategoriesRequest
{
    public string Session_Id { get; set; } = string.Empty;
    public string User_Name { get; set; } = string.Empty;
    public string Heat_Name { get; set; } = string.Empty;
    public string? Heat_Id { get; set; }
}
