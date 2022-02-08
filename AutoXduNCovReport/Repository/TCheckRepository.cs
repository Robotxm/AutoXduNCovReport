using System;
using System.Collections.Generic;
using System.Text.Json;
using System.Threading.Tasks;
using AutoXduNCovReport.Api;
using Refit;

namespace AutoXduNCovReport.Repository
{
    class TCheckRepository
    {
        private static readonly Lazy<TCheckRepository> Lazy = new(() => new TCheckRepository());
        /// <summary>
        /// Get the singleton of the repository.
        /// </summary>
        public static TCheckRepository Instance => Lazy.Value;

        private readonly ITCheckApi _api;
        private const string BaseUrl = "https://xxcapp.xidian.edu.cn/";

        private TCheckRepository()
        {
            var options = new JsonSerializerOptions
            {
                PropertyNamingPolicy = JsonNamingPolicy.CamelCase
            };
            var settings = new RefitSettings
            {
                ContentSerializer = new SystemTextJsonContentSerializer(options)
            };

            _api = RestService.For<ITCheckApi>(BaseUrl, settings);
        }

        /// <summary>
        /// Login with specified credentials.
        /// </summary>
        /// <param name="username">The username, which is always the user's student number.</param>
        /// <param name="password">The password.</param>
        /// <returns>A task represents the result, which wraps a tuple whose first element is the flag that indicates the status and second is the error message.</returns>
        public async Task<Tuple<bool, string>> Login(string username, string password)
        {
            var (code, _, message) = await _api.Login(new Model.UserInfo(username, password));

            return new Tuple<bool, string>(code == 0, message);
        }

        /// <summary>
        /// Check if the user has submitted.
        /// </summary>
        /// <returns>A task represents the status.</returns>
        public async Task<bool> CheckIsReported()
        {
            var response = await _api.GetTCheckStatus();
            
            // The 'readonly' field indicates the status
            return response.Data.Readonly;
        }

        /// <summary>
        /// Submit the given data.
        /// </summary>
        /// <param name="formData">The data to be submitted.</param>
        /// <returns>A task represents the result, which wraps a tuple whose first element is the flag that indicates the status and second is the error message.</returns>
        public async Task<Tuple<bool, string>> Submit(Dictionary<string, string> formData)
        {
            var (code, _, message) = await _api.Submit(formData);

            return new Tuple<bool, string>(code == 0, message);
        }
    }
}
