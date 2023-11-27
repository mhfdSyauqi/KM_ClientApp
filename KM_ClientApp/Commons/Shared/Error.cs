using KM_ClientApp.Models.Response;

namespace KM_ClientApp.Commons.Shared;

public sealed record Error(string Code, string? Description = null)
{
    public static readonly Error None = new(string.Empty);

    public static implicit operator Result(Error error) => Result.Failure(error);


    // Implicit GenericType Error
    public static implicit operator Result<CreatedSessionResponse>(Error error) => Result.Failure<CreatedSessionResponse>(error);
    public static implicit operator Result<GetSessionResponse>(Error error) => Result.Failure<GetSessionResponse>(error);
}
