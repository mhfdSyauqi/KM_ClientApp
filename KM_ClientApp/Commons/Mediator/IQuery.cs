using KM_ClientApp.Commons.Shared;
using MediatR;

namespace KM_ClientApp.Commons.Mediator;

public interface IQuery<TResponse> : IRequest<Result<TResponse>>
{
}
