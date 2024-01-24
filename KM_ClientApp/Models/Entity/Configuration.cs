namespace KM_ClientApp.Models.Entity;

// View_Configuration_Type_Number & View_Configuration_Type_Text 
public class Configuration
{
    public string App_Name { get; set; } = string.Empty;
    public string App_Image { get; set; } = string.Empty;

    public int Delay_Typing { get; set; }
    public int Idle_Duration { get; set; }
    public int Idle_Attempt { get; set; }
}
