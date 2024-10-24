namespace KM_ClientApp.Models.Entity;

public class EmailLog
{
    public string? To { get; set; }
    public string? Subject { get; set; }
    public string? Body { get; set; }
    public int MailId { get; set; }
    public string? Cc { get; set; }
    public string? Bcc { get; set; }
}
