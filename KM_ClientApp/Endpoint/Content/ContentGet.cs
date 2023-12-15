using FluentValidation;
using KM_ClientApp.Commons.Entity;
using KM_ClientApp.Commons.Mediator;
using KM_ClientApp.Commons.Shared;
using KM_ClientApp.Models.Request;
using KM_ClientApp.Models.Response;

namespace KM_ClientApp.Endpoint.Content;

public record GetContentQuery(BotContentRequest ContentRequest) : IQuery<BotContentResponse>;

public class GetContentQueryHandler : IQueryHandler<GetContentQuery, BotContentResponse>
{
    private readonly IContentRepository _contentRepository;
    private readonly IValidator<GetContentQuery> _validator;

    public GetContentQueryHandler(IContentRepository contentRepository, IValidator<GetContentQuery> validator)
    {
        _contentRepository = contentRepository;
        _validator = validator;
    }

    public async Task<Result<BotContentResponse>> Handle(GetContentQuery request, CancellationToken cancellationToken)
    {
        await _validator.ValidateAndThrowAsync(request, cancellationToken);

        var content = await _contentRepository.GetBotContentByIdAsync(request.ContentRequest, cancellationToken);

        if (content == null)
        {
            return Result.Failure<BotContentResponse>(new Error(
                "Content.NotFound",
                $"Content with Id : {request.ContentRequest.Searched_Id} not found"));
        }

        var response = new BotContentResponse
        {
            Id = content.Uid.ToString("N"),
            Description = content.Description,
        };

        return Result.Success(response);
    }
}

public class GetContentQueryValidator : AbstractValidator<GetContentQuery>
{
    public GetContentQueryValidator()
    {
        RuleFor(key => key.ContentRequest.Searched_Id).BeValidGuid();
    }
}

