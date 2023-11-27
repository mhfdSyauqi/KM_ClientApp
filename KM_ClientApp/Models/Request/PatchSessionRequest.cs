namespace KM_ClientApp.Models.Request;

public class PatchSessionRequest
{
    public string Id { get; set; } = string.Empty;
    public string User_Name { get; set; } = string.Empty;
    public string New_Records { get; set; } = string.Empty;
}
