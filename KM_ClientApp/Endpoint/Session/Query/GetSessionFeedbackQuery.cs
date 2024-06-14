using KM_ClientApp.Commons.Mediator;
using KM_ClientApp.Commons.Shared;
using KM_ClientApp.Models.Response;

namespace KM_ClientApp.Endpoint.Session.Query;

public record GetSessionFeedbackQuery(string UserName) : IQuery<GetSessionFeedbackResponse>;

public class GetSessionFeedbackQueryHandler : IQueryHandler<GetSessionFeedbackQuery, GetSessionFeedbackResponse>
{
    private readonly ISessionRepository _sessionRepository;

    public GetSessionFeedbackQueryHandler(ISessionRepository sessionRepository)
    {
        _sessionRepository = sessionRepository;
    }

    public async Task<Result<GetSessionFeedbackResponse>> Handle(GetSessionFeedbackQuery request, CancellationToken cancellationToken)
    {
        var session = await _sessionRepository.GetSessionFeedbackByUserNameAsync(request.UserName, cancellationToken);

        if (session == null)
        {
            return SessionErrors.NotFound;
        }

        GetSessionFeedbackResponse response = new()
        {
            Id = session.Uid.ToString("N")
        };

        return Result.Success(response);
    }
}

