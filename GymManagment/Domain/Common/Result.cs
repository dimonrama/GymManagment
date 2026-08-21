namespace GymManagment.Domain.Common
{
    public class Result
    { 
        public enum ErrorTypes
        {
            None,
            NotFound,
            Conflict,
            ServerError,
            ValidationError
        }

        public bool Success { get; }
        public string? ErrorMessage { get; }
        public ErrorTypes ErrorType { get; }
        private Result(bool success, string? errorMessage, ErrorTypes errorType)
        {
            Success = success;
            ErrorMessage = errorMessage;
            ErrorType = errorType;
        }
       
        public static Result Ok() => new Result(true, null, ErrorTypes.None);
        public static Result Fail(string errorMessage, ErrorTypes errorType) => new Result(false, errorMessage, errorType);
    }
}
