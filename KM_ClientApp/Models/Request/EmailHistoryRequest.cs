using KM_ClientApp.Models.Entity;

namespace KM_ClientApp.Models.Request;

public class EmailHistoryRequest
{
    public List<EmailHistoryItem> Histories { get; set; }
}
