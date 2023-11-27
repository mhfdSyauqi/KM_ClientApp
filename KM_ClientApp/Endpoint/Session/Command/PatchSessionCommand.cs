using FluentValidation;
using KM_ClientApp.Commons.Mediator;
using KM_ClientApp.Commons.Shared;
using KM_ClientApp.Models.Request;

namespace KM_ClientApp.Endpoint.Session.Command;

public record PatchSessionCommand(PatchSessionRequest patchSession) : ICommand;

public class PatchSessionCommandHandler : ICommandHandler<PatchSessionCommand>
{
    private readonly ISessionRepository _sessionRepository;

    public PatchSessionCommandHandler(ISessionRepository sessionRepository)
    {
        _sessionRepository = sessionRepository;
    }

    public async Task<Result> Handle(PatchSessionCommand request, CancellationToken cancellationToken)
    {
        var validator = new PatchSessionCommandValidator();
        var validationResult = validator.Validate(request);

        if (!validationResult.IsValid)
        {
            string errorMsg = validationResult.Errors.First().ErrorMessage;

            return SessionErrors.ValidationError(errorMsg);
        }

        var result = await _sessionRepository.PatchActiveSessionAsync(request.patchSession, cancellationToken);

        if (result == 0)
        {
            return SessionErrors.BadRequest;
        }

        return Result.Success();
    }
}

public class PatchSessionCommandValidator : AbstractValidator<PatchSessionCommand>
{
    public PatchSessionCommandValidator()
    {
        RuleFor(key => key.patchSession).NotNull();
        RuleFor(key => key.patchSession.New_Records).NotEmpty().WithMessage("Please Prove Correct Records");
        RuleFor(key => key.patchSession.Id).Must(BeValidGuid).WithMessage("Id Is Incorect");
        RuleFor(key => key.patchSession.User_Name).Must(BeValidUserName).WithMessage("Unauthorize user");
    }

    private bool BeValidGuid(string id)
    {
        return Guid.TryParse(id, out var guid);
    }
    private bool BeValidUserName(string userName)
    {
        if (userName != null && userName != "NotAuthUser")
        {
            return true;
        }

        return false;
    }
}
