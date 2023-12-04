using System.Reflection;

namespace KM_ClientApp.Commons.Shared;

public class Result
{
    public Result(bool isSuccess, Error error)
    {
        if (isSuccess && error != Error.None || !isSuccess && error == Error.None)
        {
            throw new ArgumentException("Invalid Type Of Error", nameof(error));
        }

        IsSuccess = isSuccess;
        Error = error;

    }

    public bool IsSuccess { get; }

    public bool IsFailure => !IsSuccess;

    public Error Error { get; }

    public static Result Success() => new(true, Error.None);
    public static Result<T> Success<T>(T value) => new(true, Error.None, value);

    public static Result Failure(Error error) => new(false, error);
    public static Result<T> Failure<T>(Error error) => new(false, error, default);
}

public class Result<T> : Result
{
    public T? Value { get; }

    protected internal Result(bool isSuccess, Error error, T? value) : base(isSuccess, error)
    {
        if (isSuccess && value == null || !isSuccess && value != null)
        {
            throw new ArgumentException("Invalid Type Of Error", nameof(error));
        }

        Value = value;
    }

    public Response CreateResponseObject()
    {
        Data dataObj = new();

        if (Value != null)
        {
            var sourcePropertiesType = Value.GetType().Name;
            List<PropertyInfo> destinationProperties = dataObj.GetType().GetProperties().ToList();
            PropertyInfo? destinationProperty = destinationProperties.Find(prop => prop.PropertyType.Name == sourcePropertiesType);

            destinationProperty?.SetValue(dataObj, Value, null);
        }
        return new Response(dataObj);
    }
}
