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
    private readonly IValidator<EndSessionCommand> _validator;

    public EndSessionCommandHandler(ISessionRepository sessionRepository, IValidator<EndSessionCommand> validator)
    {
        _sessionRepository = sessionRepository;
        _validator = validator;
    }

    public async Task<Result> Handle(EndSessionCommand request, CancellationToken cancellationToken)
    {
        await _validator.ValidateAndThrowAsync(request, cancellationToken);

        var result = await _sessionRepository.EndSessionAsync(request.EndSession, cancellationToken);

        if (result < 0)
        {
            return SessionErrors.Invalid;
        }

        return Result.Success();
    }
}

public class EndSessionValidator : AbstractValidator<EndSessionCommand>
{
    public EndSessionValidator()
    {
        RuleFor(key => key.EndSession).NotNull();
        RuleFor(key => key.EndSession.Id).BeValidGuid();
        RuleFor(key => key.EndSession.User_Name).BeValidUserName();
    }
}
