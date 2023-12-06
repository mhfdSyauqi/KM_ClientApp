namespace KM_ClientApp.Commons.Entity;

public class HttpResponseError
{
    public HttpResponseError(string detail)
    {
        Detail = detail;
    }
    public string Error { get; init; } = "An error occurred while processing your request";
    public string Detail { get; init; }
}
