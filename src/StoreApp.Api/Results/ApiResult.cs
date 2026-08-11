namespace StoreApp.Api.Results
{
    public class ApiResult
    {
        public bool IsSuccess { get; init; }

        public string? ErrorMessage { get; init; }

        public static ApiResult Success()
        {
            return new ApiResult
            {
                IsSuccess = true
            };
        }

        public static ApiResult Failure(string errorMessage)
        {
            return new ApiResult
            {
                IsSuccess = false,
                ErrorMessage = errorMessage
            };
        }
    }

    public class ApiResult<T> : ApiResult
    {
        public T? Data { get; init; }

        public static ApiResult<T> Success(T data)
        {
            return new ApiResult<T>
            {
                IsSuccess = true,
                Data = data
            };
        }

        public static new ApiResult<T> Failure(string errorMessage)
        {
            return new ApiResult<T>
            {
                IsSuccess = false,
                ErrorMessage = errorMessage
            };
        }
    }
}