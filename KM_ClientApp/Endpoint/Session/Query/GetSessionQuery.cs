using KM_ClientApp.Commons.Mediator;
using KM_ClientApp.Commons.Shared;
using KM_ClientApp.Models.Response;

namespace KM_ClientApp.Endpoint.Session.Query;

public record GetSessionQuery(string UserName) : IQuery<GetSessionResponse>;

public class GetSessionQueryHandler : IQueryHandler<GetSessionQuery, GetSessionResponse>
{
    private readonly ISessionRepository _sessionRepository;

    public GetSessionQueryHandler(ISessionRepository sessionRepository)
    {
        _sessionRepository = sessionRepository;
    }

    public async Task<Result<GetSessionResponse>> Handle(GetSessionQuery request, CancellationToken cancellationToken)
    {
        var session = await _sessionRepository.GetSessionByUserNameAsync(request.UserName, cancellationToken);

        if (session == null)
        {
            return SessionErrors.NotFound;
        }

        GetSessionResponse response = new()
        {
            Id = session.Uid.ToString("N"),
            Is_Active = session.Is_Active,
            Has_Feedback = session.Has_Feedback,
            Records = session.Records
        };

        return Result.Success(response);
    }
}

