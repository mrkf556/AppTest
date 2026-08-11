using Microsoft.AspNetCore.Mvc;
using StoreApp.Application.Abstractions.Results;

namespace StoreApp.Api.Results
{
    public static class ApiResultExtensions
    {
        public static ActionResult ToApiResult(
            this ControllerBase controller,
            ServiceResult result)
        {
            if (result.IsSuccess)
            {
                return controller.Ok(
                    ApiResult.Success());
            }

            return controller.BadRequest(
                ApiResult.Failure(
                    result.ErrorMessage ?? "عملیات ناموفق بود."));
        }

        public static ActionResult<ApiResult<T>> ToApiResult<T>(
            this ControllerBase controller,
            ServiceResult<T> result)
        {
            if (result.IsSuccess)
            {
                return controller.Ok(
                    ApiResult<T>.Success(result.Data!));
            }

            return controller.BadRequest(
                ApiResult<T>.Failure(
                    result.ErrorMessage ?? "عملیات ناموفق بود."));
        }
    }
}