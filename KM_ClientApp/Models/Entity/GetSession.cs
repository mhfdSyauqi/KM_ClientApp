namespace KM_ClientApp.Models.Entity;

// * View_Active_User_Session_Record
public class GetSession
{
    public Guid Uid { get; set; }
    public bool Is_Active { get; set; }
    public bool Has_Feedback { get; set; }
    public string Records { get; set; } = string.Empty;
}

