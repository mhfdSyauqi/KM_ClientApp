namespace KM_ClientApp.Models.Entity;

public class UserFeedback
{
    public Guid Session_Id { get; init; }
    public int Rating { get; init; }
    public string? Remark { get; init; }
    public string User_Name { get; init; } = string.Empty;
}
