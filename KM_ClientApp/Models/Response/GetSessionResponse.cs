namespace KM_ClientApp.Models.Response;

public class GetSessionResponse
{
    public string Id { get; set; } = string.Empty;
    public bool Is_Active { get; set; }
    public bool Has_Feedback { get; set; }
    public string Records { get; set; } = string.Empty;
}
