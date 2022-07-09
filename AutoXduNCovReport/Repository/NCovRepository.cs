using System;
using System.Collections.Generic;
using System.Text.Json;
using System.Text.RegularExpressions;
using System.Threading.Tasks;
using AutoXduNCovReport.Api;
using Refit;

namespace AutoXduNCovReport.Repository
{
    /// <summary>
    /// Wraps the network operation of report system.
    /// </summary>
    class NCovRepository
    {
        private static readonly Lazy<NCovRepository> Lazy = new(() => new NCovRepository());
        /// <summary>
        /// Get the singleton of the repository.
        /// </summary>
        public static NCovRepository Instance => Lazy.Value;

        private readonly INCovApi _api;
        private const string BaseUrl = "https://xxcapp.xidian.edu.cn/";

        private NCovRepository()
        {
            var options = new JsonSerializerOptions
            {
                PropertyNamingPolicy = JsonNamingPolicy.CamelCase
            };
            var settings = new RefitSettings
            {
                ContentSerializer = new SystemTextJsonContentSerializer(options)
            };

            _api = RestService.For<INCovApi>(BaseUrl, settings);
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
        /// Get the old information submitted before.
        /// </summary>
        /// <returns>A task represents the information, which wraps a dictionary. The content in the task could be null when failed to parse.</returns>
        public async Task<Dictionary<string, object>?> GetOldInfo()
        {
            var rawData = await _api.GetOldInfo();
            // Find the old information
            var match = Regex.Match(rawData, "var def = ([\\s\\S]*?);\n", RegexOptions.Singleline);
            if (!match.Success)
                return null;

            var initParams = match.Groups[1].Value;
            // Process empty data
            // Empty data is caused by missing report
            if (initParams.Contains('\n'))
            {
                // When the data is empty, keys of 'def' are not quoted.
                // We need to do something to convert it to JSON string.
                // Painful.
                initParams = Regex.Replace(initParams, "(\\s+)(.*?):", "$1\"$2\":"); 
                initParams = Regex.Replace(initParams, ":(.*?)(')", ":$1\"");
                initParams = Regex.Replace(initParams, "'(.*?),", "\"$1,");
            }
            var paramsDict = JsonSerializer.Deserialize<Dictionary<string, object>>(initParams);

            return paramsDict;
        }

        /// <summary>
        /// Submit the given data.
        /// </summary>
        /// <param name="formData">The data to be submitted.</param>
        /// <returns>A task represents the result, which wraps a tuple whose first element is the flag that indicates the status and second is the error message.</returns>
        public async Task<Tuple<bool, string>> Submit(Dictionary<string, string?> formData)
        {
            var response = await _api.Submit(formData);

            return new Tuple<bool, string>(response.Code == 0, response.Message);
        }

        /// <summary>
        /// Check if the user has submitted.
        /// </summary>
        /// <returns>A task represents the status.</returns>
        public async Task<bool> CheckIsReported()
        {
            var response = await _api.GetOldInfo();
            // The 'hasFlag' field indicated the status
            return response.Contains("hasFlag: '1'");
        }
    }
}