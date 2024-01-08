using KM_ClientApp.Commons.Mediator;
using KM_ClientApp.Commons.Shared;
using KM_ClientApp.Models.Entity;
using KM_ClientApp.Models.Request;
using KM_ClientApp.Models.Response;

namespace KM_ClientApp.Endpoint.Message;

public record GetBotMessageQuery(BotMessageType Type, BotMessageRequest MessageRequest) : IQuery<List<BotMessageResponse>>;

public class GetBotMessageQueryHandler : IQueryHandler<GetBotMessageQuery, List<BotMessageResponse>>
{
    private readonly IMessageRepository _messageRepository;

    public GetBotMessageQueryHandler(IMessageRepository messageRepository)
    {
        _messageRepository = messageRepository;
    }

    public async Task<Result<List<BotMessageResponse>>> Handle(GetBotMessageQuery request, CancellationToken cancellationToken)
    {
        var message = await _messageRepository.GetBotMessageAsync(request.Type, request.MessageRequest, cancellationToken);
        var msgCount = message.Count();

        if (msgCount == 0)
        {
            return Result.Failure<List<BotMessageResponse>>(new Error(
                "Message.NotFound",
                "There is no message found in databases"));
        }

        var response = message.Select(row => new BotMessageResponse
        {
            Type = row.Sequence == msgCount ? "desc" : "message",
            Text = row.Contents,
        }).ToList();


        return Result.Success(response);
    }
}
