namespace Kumadio.Core.Common
{
    public class Result
    {
        public bool IsSuccess { get; }
        public Error? Error { get; }

        protected Result(bool isSuccess, Error? error)
        {
            if (isSuccess && error != null)
            {
                throw new InvalidOperationException();
            }

            if (!isSuccess && error == null)
            {
                throw new InvalidOperationException();
            }

            IsSuccess = isSuccess;
            Error = error;
        }

        public bool IsFailure => !IsSuccess;

        public static Result Success() => new(true, null);

        public static Result Fail(Error error) => new Result(false, error);

        public static implicit operator Result(Error error) => Fail(error);

    }

    public class Result<T> : Result
    {
        public T? Value { get; }

        private Result(bool isSuccess, Error? error, T? value)
            : base(isSuccess, error)
        {
            Value = value;
        }

        public static Result<T> Success(T value) => value == null
            ? throw new InvalidOperationException("The value of successfull result must have value.") 
            : new Result<T>(true, null, value);
        public static new Result<T> Fail(Error error) => new Result<T>(false, error, default);

        // Implicit operators
        public static implicit operator Result<T>(T value) => Success(value);
        public static implicit operator Result<T>(Error error) => Fail(error);
    }
}
