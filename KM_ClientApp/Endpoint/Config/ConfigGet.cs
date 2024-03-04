using KM_ClientApp.Commons.Mediator;
using KM_ClientApp.Commons.Shared;
using KM_ClientApp.Models.Response;

namespace KM_ClientApp.Endpoint.Config;

public record GetConfigurationQuery : IQuery<ConfigurationResponse>;

public class GetConfigurationHandler : IQueryHandler<GetConfigurationQuery, ConfigurationResponse>
{
    private readonly IConfigRepository _configRepository;

    public GetConfigurationHandler(IConfigRepository configRepository)
    {
        _configRepository = configRepository;
    }

    public async Task<Result<ConfigurationResponse>> Handle(GetConfigurationQuery request, CancellationToken cancellationToken)
    {
        var config = await _configRepository.GetAppConfigurationAsync(cancellationToken);

        if (config == null)
        {
            return Result.Failure<ConfigurationResponse>(new Error(
                "Config.NotFound",
                "Theres is no configuration found in database"));
        }

        ConfigurationResponse response = new()
        {
            App_Image = config.App_Image,
            App_Name = config.App_Name,
            Article_Link = config.Article_Link,
            Delay_Typing = config.Delay_Typing,
            Idle_Attempt = config.Idle_Attempt,
            Idle_Duration = config.Idle_Duration
        };

        return Result.Success(response);
    }
}


