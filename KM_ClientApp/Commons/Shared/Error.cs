using KM_ClientApp.Models.Response;

namespace KM_ClientApp.Commons.Shared;

public sealed record Error(string Code, string? Description = null)
{
    public static readonly Error None = new(string.Empty);

    public static implicit operator Result(Error error) => Result.Failure(error);
    public static implicit operator Result<CreatedSessionResponse>(Error error) => Result.Failure<CreatedSessionResponse>(error);
    public static implicit operator Result<GetSessionResponse>(Error error) => Result.Failure<GetSessionResponse>(error);
    public static implicit operator Result<GetSessionFeedbackResponse>(Error error) => Result.Failure<GetSessionFeedbackResponse>(error);
    public static implicit operator Result<GetCategoriesResponse>(Error error) => Result.Failure<GetCategoriesResponse>(error);
    public static implicit operator Result<SearchCategoriesResponse>(Error error) => Result.Failure<SearchCategoriesResponse>(error);
    public static implicit operator Result<SuggestionCategoriesResponse>(Error error) => Result.Failure<SuggestionCategoriesResponse>(error);
}
