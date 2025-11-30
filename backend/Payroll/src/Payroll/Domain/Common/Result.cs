namespace Payroll.Domain.Common;

public class Result
{
    public bool IsSuccess { get; }
    public bool IsFailure => !IsSuccess;
    public DomainError? Error { get; }

    protected Result(bool isSuccess, DomainError? error)
    {
        IsSuccess = isSuccess;
        Error = error;
    }

    public static Result Success() => new(true, null);

    public static Result Failure(DomainError error) => new(false, error);

    public static Result<T> Success<T>(T value) => new(value, true, null);

    public static Result<T> Failure<T>(DomainError error) => new(default!, false, error);
}

public sealed class Result<T> : Result
{
    public T Value { get; }

    internal Result(T value, bool isSuccess, DomainError? error)
        : base(isSuccess, error) => Value = value;
}
