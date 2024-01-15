using KM_ClientApp.Models.Response;
using System.Text.Json.Serialization;

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
    [JsonIgnore(Condition = JsonIgnoreCondition.WhenWritingNull)]
    public ConfigurationResponse? Configurations { get; set; }

    [JsonIgnore(Condition = JsonIgnoreCondition.WhenWritingNull)]
    public GetSessionResponse? Session { get; set; }

    [JsonIgnore(Condition = JsonIgnoreCondition.WhenWritingNull)]
    public List<BotMessageResponse>? Messages { get; set; }

    [JsonIgnore(Condition = JsonIgnoreCondition.WhenWritingNull)]
    public GetCategoriesResponse? Categories { get; set; }

    [JsonIgnore(Condition = JsonIgnoreCondition.WhenWritingNull)]
    public SearchCategoriesResponse? Searched_Categories { get; set; }

    [JsonIgnore(Condition = JsonIgnoreCondition.WhenWritingNull)]
    public SuggestionCategoriesResponse? Suggested_Categories { get; set; }

    [JsonIgnore(Condition = JsonIgnoreCondition.WhenWritingNull)]
    public BotContentResponse? Content { get; set; }

    [JsonIgnore(Condition = JsonIgnoreCondition.WhenWritingNull)]
    public ReferenceCategoriesResponse? Reference_Categories { get; set; }

    [JsonIgnore(Condition = JsonIgnoreCondition.WhenWritingNull)]
    public ReAskedResponse? ReAsked { get; set; }
}

