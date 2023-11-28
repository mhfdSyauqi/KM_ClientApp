using KM_ClientApp.Commons.Mediator;
using KM_ClientApp.Commons.Shared;
using KM_ClientApp.Models.Response;

namespace KM_ClientApp.Endpoint.Session.Command;

public record AddSessionCommand(string userName) : ICommand<CreatedSessionResponse>;

public class AddSessionCommandHandler : ICommandHandler<AddSessionCommand, CreatedSessionResponse>
{
    private readonly ISessionRepository _sessionRepository;

    public AddSessionCommandHandler(ISessionRepository sessionRepository)
    {
        _sessionRepository = sessionRepository;
    }

    public async Task<Result<CreatedSessionResponse>> Handle(AddSessionCommand request, CancellationToken cancellationToken)
    {
        if (request.userName == "NotAuthUser")
        {
            return SessionErrors.NotAuthorized;
        }

        var newSession = await _sessionRepository.AddEmptySessionAsync(request.userName, cancellationToken);

        if (newSession == null)
        {
            return SessionErrors.Exist;
        }

        CreatedSessionResponse response = new()
        {
            Created_Id = newSession.Uid.ToString("N")
        };

        return Result.Success(response);
    }
}
