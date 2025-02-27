﻿using FluentValidation;
using KM_ClientApp.Commons.Entity;
using KM_ClientApp.Commons.Mediator;
using KM_ClientApp.Commons.Shared;
using KM_ClientApp.Models.Request;

namespace KM_ClientApp.Endpoint.Session.Command;

public record PatchSessionCommand(PatchSessionRequest patchSession) : ICommand;

public class PatchSessionCommandHandler : ICommandHandler<PatchSessionCommand>
{
    private readonly ISessionRepository _sessionRepository;
    private readonly IValidator<PatchSessionCommand> _validator;

    public PatchSessionCommandHandler(ISessionRepository sessionRepository, IValidator<PatchSessionCommand> validator)
    {
        _sessionRepository = sessionRepository;
        _validator = validator;
    }

    public async Task<Result> Handle(PatchSessionCommand request, CancellationToken cancellationToken)
    {
        await _validator.ValidateAndThrowAsync(request, cancellationToken);

        var result = await _sessionRepository.PatchActiveSessionAsync(request.patchSession, cancellationToken);

        if (result < 0)
        {
            return SessionErrors.Invalid;
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
        RuleFor(key => key.patchSession.Id).BeValidGuid();
        RuleFor(key => key.patchSession.User_Name).BeValidUserName();
    }
}
