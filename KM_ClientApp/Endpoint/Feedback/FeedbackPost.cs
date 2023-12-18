using FluentValidation;
using KM_ClientApp.Commons.Entity;
using KM_ClientApp.Commons.Mediator;
using KM_ClientApp.Commons.Shared;
using KM_ClientApp.Models.Request;

namespace KM_ClientApp.Endpoint.Feedback;

public record FeedbackPostCommand(UserFeedbackRequest FeedbackRequest) : ICommand;

public record FeedbackPostCommandHandler : ICommandHandler<FeedbackPostCommand>
{
    private readonly IFeedbackRepository _feedbackRepository;
    private readonly IValidator<FeedbackPostCommand> _validator;

    public FeedbackPostCommandHandler(IFeedbackRepository feedbackRepository, IValidator<FeedbackPostCommand> validator)
    {
        _feedbackRepository = feedbackRepository;
        _validator = validator;
    }

    public async Task<Result> Handle(FeedbackPostCommand request, CancellationToken cancellationToken)
    {
        await _validator.ValidateAndThrowAsync(request, cancellationToken);

        await _feedbackRepository.AddUserFeedbackAsync(request.FeedbackRequest, cancellationToken);

        return Result.Success();
    }
}


public class FeedbackPostCommandValidator : AbstractValidator<FeedbackPostCommand>
{
    public FeedbackPostCommandValidator()
    {
        RuleFor(key => key.FeedbackRequest.Session_Id).BeValidGuid();
        RuleFor(key => key.FeedbackRequest.User_Name).BeValidUserName();
        When(props => props.FeedbackRequest.Rating < 3, () =>
        {
            RuleFor(key => key.FeedbackRequest.Remark).NotEmpty().NotNull();
        });
    }
}
