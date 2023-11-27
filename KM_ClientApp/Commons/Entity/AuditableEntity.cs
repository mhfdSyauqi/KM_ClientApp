namespace KM_ClientApp.Commons.Entity;

public abstract class AuditableEntity
{
    public string Create_By { get; set; } = "system";
    public DateTime Create_At { get; set; }
    public string? Modified_By { get; set; }
    public DateTime? Modified_At { get; set; }
}
