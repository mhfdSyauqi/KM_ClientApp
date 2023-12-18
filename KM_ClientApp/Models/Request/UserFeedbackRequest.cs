namespace KM_ClientApp.Models.Request;

public class UserFeedbackRequest
{
    public string Session_Id { get; set; } = string.Empty;
    public int Rating { get; set; }
    public string? Remark { get; set; }
    public string User_Name { get; set; } = string.Empty;
}
