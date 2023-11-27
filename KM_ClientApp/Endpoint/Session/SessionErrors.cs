using KM_ClientApp.Commons.Shared;

namespace KM_ClientApp.Endpoint.Session;

public static class SessionErrors
{
    public static readonly Error NotFound = new(
        "Session.NotFound",
        "There is no user session in database");

    public static readonly Error BadRequest = new(
        "Session.BadRequest",
        "This user is not authorize!");

    public static readonly Error Exist = new(
        "Session.Exist",
        "Failed to add new session, becase session already exist in database");

    public static Error ValidationError(string? error)
    {
        return new("Session.ValidationError", error);
    }
}

