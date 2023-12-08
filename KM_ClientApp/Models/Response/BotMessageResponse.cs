namespace KM_ClientApp.Models.Response;

public class BotMessageResponse
{
    public string Type { get; set; } = string.Empty;
    public string Text { get; set; } = string.Empty;
    public DateTime Time { get; init; } = DateTime.Now;
}
