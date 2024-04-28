namespace KM_ClientApp.Models.Request;

public class MailHelpdeskRequest
{
    public string Session_Id { get; set; } = string.Empty;
    public string Message { get; set; } = string.Empty;
    public string Send_By { get; set; } = string.Empty;
}
