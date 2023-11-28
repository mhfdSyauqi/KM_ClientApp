namespace KM_ClientApp.Models.Request;

public class EndSessionRequest
{
    public string Id { get; set; } = string.Empty;
    public string User_Name { get; set; } = string.Empty;
    public string? Ended_By { get; set; }
}
