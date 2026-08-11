using System;
using System.Collections.Generic;
using System.Text;


///یک مدل کلی برای result
namespace StoreApp.Application.Abstractions.Results
{
 
    //برای زمانی که دیتا خروجی فقط برای ما success یا error ان مهم هست منظور نتیحه خروجی انجام کار مهم هست
    public class ServiceResult
    {
        public bool IsSuccess { get; init; }

        public string? ErrorMessage { get; init; }

        public static ServiceResult Success()
        {
            return new ServiceResult
            {
                IsSuccess = true
            };
        }

        public static ServiceResult Failure(string errorMessage)
        {
            return new ServiceResult
            {
                IsSuccess = false,
                ErrorMessage = errorMessage
            };
        }
    }
    ///برای زمانی که علاوه بر نتیحه درخواست دیتا خروحی هم در نظر داریم 
    public class ServiceResult<T> : ServiceResult
    {
        public T? Data { get; init; }

        public static ServiceResult<T> Success(T data)
        {
            return new ServiceResult<T>
            {
                IsSuccess = true,
                Data = data
            };
        }

        public static new ServiceResult<T> Failure(string errorMessage)
        {
            return new ServiceResult<T>
            {
                IsSuccess = false,
                ErrorMessage = errorMessage
            };
        }
    }
}
