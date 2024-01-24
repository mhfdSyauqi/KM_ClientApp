namespace KM_ClientApp.Models.Entity;

public class EmailHistoryConfig
{
    public bool MAIL_HISTORY_STATUS { get; set; }
    public string MAIL_HISTORY_FROM { get; set; } = string.Empty;
    public string MAIL_HISTORY_SUBJECT { get; set; } = string.Empty;
    public string MAIL_CONFIG_USERNAME { get; set; } = string.Empty;
    public string MAIL_CONFIG_PASSWORD { get; set; } = string.Empty;
    public string MAIL_CONFIG_SERVER { get; set; } = string.Empty;
    public int MAIL_CONFIG_PORT { get; set; }
}
