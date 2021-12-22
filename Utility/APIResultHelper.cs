﻿using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace XMUer.Utility
{
    public class APIResultHelper
    {
        /*
        200级别，表示成功：

        200 - OK
        201 - Created，表示资源创建成功了
        204 - No content，成功执行，但是不应该返回任何东西

        400级别，表示客户端引起的错误：

        400 - Bad request，表示API的消费者发送到服务器的请求是错误的
        401 - Unauthorized，表示没有权限
        403 - Forbidden，表示用户验证成功，但是该用户仍然无法访问该资源
        404 - Not found，表示请求的资源不存在
        405 - Method not allowed，这就是当我们尝试发送请求给某个资源时，使用的HTTP方法却是不允许的，例如使用POST api/countries, 而该资源只实现了 GET，所以POST不被允许
        406 - Not acceptable，这里涉及到了media type，例如API消费者请求的是application/xml格式的media type，而API只支持application/json
        409 - Conflict，表示该请求无法完成，因为请求与当前资源的状态有冲突，例如你编辑某个资源数据以后，该资源又被其它人更新了，这时你再PUT你的数据就会出现409错误；有时也用在尝试创建资源时该资源已存在的情况。
        415 - Unsupported media type，这个和406正好返回来，比如说我向服务器提交数据的media type是xml的，而服务器只支持json，那么就会返回415
        422 - Unprocessable entity，表示请求的格式没问题，但是语义有错误，例如实体验证错误。

        500级别，服务器错误：

        500 - Internal server error，这表示是服务器发生了错误
        */
        public static APIResult Success(int code, string msg, bool result)
        {
            return new APIResult
            {
                Code = code,
                Msg = msg,
                Result = result,
                Data = null,
                Total = 0
            };
        }
        public static APIResult Success(int code, string msg, bool result,dynamic data)
        {
            return new APIResult
            {
                Code = code,
                Msg = msg,
                Result = result,
                Data = data,
                Total = 0
            };
        }
        public static APIResult Success(int code, string msg, bool result, dynamic data, int total)
        {
            return new APIResult
            {
                Code = code,
                Msg = msg,
                Result = result,
                Data = data,
                Total = total
            };
        }
        public static APIResult Error(int code,string msg, bool result)
        {
            return new APIResult
            {
                Code = code,
                Msg = msg,
                Result = result,
                Data = null,
                Total = 0
            };
        }
        public static APIResult Error(int code, string msg, bool result,dynamic data)
        {
            return new APIResult
            {
                Code = code,
                Msg = msg,
                Result = result,
                Data = data,
                Total = 0
            };
        }
    }
}
