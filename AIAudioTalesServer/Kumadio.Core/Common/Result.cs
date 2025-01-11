namespace Kumadio.Core.Common
{
    public class Result<T>
    {
        public bool IsSuccess { get; set; } = true;
        public T? Value { get; set; }
        public string? ErrorMessage { get; set; }
    
        public static Result<T> Success(T value)
        {
            return new() { Value = value };
        }

        public static Result<T> Fail(string errorMessage)
        {
            return new() { IsSuccess = false, ErrorMessage = errorMessage };
        }
    }
}
