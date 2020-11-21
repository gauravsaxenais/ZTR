namespace ZTR.Framework.Service
{
    using System;
    using ZTR.Framework.Business;
    using Microsoft.AspNetCore.Http;
    using Microsoft.AspNetCore.Mvc;

    public static class ManagerResponseExtensions
    {
        public static ObjectResult ToStatusCode<TErrorCode>(this ManagerResponseBase<TErrorCode> managerResponse, int successCode = StatusCodes.Status200OK, int errorCode = StatusCodes.Status400BadRequest)
            where TErrorCode : struct, Enum
        {
            if (managerResponse.HasError)
            {
                return new ObjectResult(managerResponse)
                {
                    StatusCode = errorCode
                };
            }
            else
            {
                return new ObjectResult(managerResponse)
                {
                    StatusCode = successCode
                };
            }
        }
    }
}
