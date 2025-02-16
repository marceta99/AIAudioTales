namespace Kumadio.Core.Common
{
    public class Result
    {
        protected static readonly Error NoError = new Error("_NO_ERROR", "No error");

        public bool IsSuccess { get; }
        public bool IsFailure => !IsSuccess;
        public Error Error { get; }

        protected Result(bool isSuccess, Error error)
        {
            if (isSuccess && error != NoError)
            {
                throw new InvalidOperationException(
                    "Cannot provide a non‐NoError object for success result.");
            }
            if (!isSuccess && error == NoError)
            {
                throw new InvalidOperationException(
                    "Must provide a real Error object for failure result.");
            }

            IsSuccess = isSuccess;
            Error = error;
        }

        public static Result Success() => new Result(true, NoError);
        public static Result Fail(Error error) => new Result(false, error);

        // Implicit operator
        public static implicit operator Result(Error error) => Fail(error);
    }

    public class Result<T> : Result
    {
        public T Value { get; }

        private Result(bool isSuccess, Error error, T value)
            : base(isSuccess, error)
        {
            if (isSuccess && value == null)
            {
                throw new InvalidOperationException("Value cannot be null on success.");
            }
            Value = value;
        }

        public static Result<T> Success(T value)
        {
            if (value == null) throw new InvalidOperationException("Value cannot be null on success.");
            return new Result<T>(true, NoError, value);
        }
        public static Result<T> Fail(Error error) => new Result<T>(false, error, default!);

        // Implicit operators
        public static implicit operator Result<T>(T value) => Success(value);
        public static implicit operator Result<T>(Error error) => Fail(error);
    }
}