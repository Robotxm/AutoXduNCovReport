using System.Collections.Generic;
using System.Threading.Tasks;
using AutoXduNCovReport.Model;
using Refit;

namespace AutoXduNCovReport.Api
{
    /// <summary>
    /// Network API for COVID-19 daily 3-time report system (a.k.a '晨午晚检')
    /// </summary>
    [Headers(
        "User-Agent: Mozilla/5.0 (Linux; Android 8.0.0; MI 5 Build/OPR1.170623.032; wv) AppleWebKit/537.36 (KHTML, like Gecko) Version/4.0 Chrome/66.0.3359.126 MQQBrowser/6.2 TBS/45016 Mobile Safari/537.36 MMWEBID/2993 MicroMessenger/7.0.9.1560(0x27000935) Process/tools NetType/WIFI Language/zh_CN ABI/arm64",
        "Connection: keep-alive")]
    interface ITCheckApi
    {
        /// <summary>
        /// Login to the system with the specified credential.
        /// </summary>
        /// <param name="userInfo">The credentials, which contains username and password.</param>
        /// <returns>A task that represents the asynchronous login operation.</returns>
        [Post("/uc/wap/login/check")]
        Task<BaseResponse<object?>> Login([Body(BodySerializationMethod.UrlEncoded)]
            UserInfo userInfo);

        /// <summary>
        /// Check the tcheck status
        /// </summary>
        /// <returns>A task that represents the fetching operation, which wraps the tcheck information.</returns>
        [Get("/xisuncov/wap/open-report/index")]
        Task<BaseResponse<TCheckData>> GetTCheckStatus();

        /// <summary>
        /// Submitted the given data.
        /// </summary>
        /// <param name="formData">The data to be submitted.</param>
        /// <returns>A task that represents the submitting operation.</returns>
        [Post("/xisuncov/wap/open-report/save")]
        Task<BaseResponse<object?>> Submit([Body(BodySerializationMethod.UrlEncoded)]
            Dictionary<string, string> formData);
    }
}
