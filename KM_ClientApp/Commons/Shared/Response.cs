using KM_ClientApp.Models.Response;

namespace KM_ClientApp.Commons.Shared;

public class Response
{
    public Response(Data newData)
    {
        Data = newData;
    }
    public Data Data { get; }
}

public class Data
{
    public ConfigurationResponse? Configurations { get; set; }
    public GetSessionResponse? Session { get; set; }
    public List<BotMessageResponse>? Messages { get; set; }

}

