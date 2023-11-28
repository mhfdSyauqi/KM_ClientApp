namespace KM_ClientApp.Models.Entity;

public class EndSession
{
    public Guid Uid { get; set; }
    public string User_Name { get; set; } = string.Empty;
    public string? Ended_By { get; set; }
}
