using FluentValidation;
using KM_ClientApp.Commons.Entity;
using KM_ClientApp.Commons.Mediator;
using KM_ClientApp.Commons.Shared;
using KM_ClientApp.Models.Response;

namespace KM_ClientApp.Endpoint.Session.Command;

public record AddSessionCommand(string userName) : ICommand<CreatedSessionResponse>;

public class AddSessionCommandHandler : ICommandHandler<AddSessionCommand, CreatedSessionResponse>
{
    private readonly ISessionRepository _sessionRepository;
    private readonly IValidator<AddSessionCommand> _validator;

    public AddSessionCommandHandler(ISessionRepository sessionRepository, IValidator<AddSessionCommand> validator)
    {
        _sessionRepository = sessionRepository;
        _validator = validator;
    }

    public async Task<Result<CreatedSessionResponse>> Handle(AddSessionCommand request, CancellationToken cancellationToken)
    {
        await _validator.ValidateAndThrowAsync(request, cancellationToken);

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

    public class AddSessionCommandValidator : AbstractValidator<AddSessionCommand>
    {
        public AddSessionCommandValidator()
        {
            RuleFor(key => key.userName).BeValidUserName();
        }
    }
}
