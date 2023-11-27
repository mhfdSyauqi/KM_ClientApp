using KM_ClientApp.Commons.Shared;
using MediatR;

namespace KM_ClientApp.Commons.Mediator;

public interface ICommand : IRequest<Result>
{
}

public interface ICommand<TResponse> : IRequest<Result<TResponse>>
{
}
