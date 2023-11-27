using KM_ClientApp.Commons.Shared;
using MediatR;

namespace KM_ClientApp.Commons.Mediator;

public interface IQueryHandler<TQuery, TResponse>
    : IRequestHandler<TQuery, Result<TResponse>>
    where TQuery : IQuery<TResponse>
{
}
