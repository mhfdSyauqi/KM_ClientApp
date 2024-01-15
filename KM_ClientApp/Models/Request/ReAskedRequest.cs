namespace KM_ClientApp.Models.Request;

public class ReAskedRequest
{
    public DateTime Create_At { get; init; } = DateTime.Now;
    public string Create_By { get; set; } = string.Empty;
    public string Category_Id { get; set; } = string.Empty;
}
