using FluentValidation;
using KM_ClientApp.Commons.Entity;
using KM_ClientApp.Commons.Mediator;
using KM_ClientApp.Commons.Shared;
using KM_ClientApp.Models.Request;

namespace KM_ClientApp.Endpoint.Session.Command;

public record EndSessionCommand(EndSessionRequest EndSession) : ICommand;


public class EndSessionCommandHandler : ICommandHandler<EndSessionCommand>
{
    private readonly ISessionRepository _sessionRepository;

    public EndSessionCommandHandler(ISessionRepository sessionRepository)
    {
        _sessionRepository = sessionRepository;
    }

    public async Task<Result> Handle(EndSessionCommand request, CancellationToken cancellationToken)
    {
        var validator = new EndSessionValidator();
        var validationResult = await validator.ValidateAsync(request, cancellationToken);

        if (!validationResult.IsValid)
        {
            string errorMsg = validationResult.Errors.First().ErrorMessage;
            return SessionErrors.ValidationError(errorMsg);
        }

        var result = await _sessionRepository.EndSessionAsync(request.EndSession, cancellationToken);

        if (result < 0)
        {
            return SessionErrors.NotFound;
        }

        return Result.Success();
    }
}

public class EndSessionValidator : AbstractValidator<EndSessionCommand>
{
    public EndSessionValidator()
    {
        RuleFor(x => x.EndSession).NotNull();
        RuleFor(x => x.EndSession.Id).BeValidGuid();
        RuleFor(x => x.EndSession.User_Name).BeValidUserName();
    }
}
