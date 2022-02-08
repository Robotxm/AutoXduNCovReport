using System;
using System.Threading.Tasks;
using AutoXduNCovReport.Api;
using Refit;

namespace AutoXduNCovReport.Repository
{
    class ServerchanRepository
    {
        private static readonly Lazy<ServerchanRepository> Lazy = new(() => new ServerchanRepository());

        /// <summary>
        /// Get the singleton of the repository.
        /// </summary>
        public static ServerchanRepository Instance => Lazy.Value;

        private readonly IServerchanApi _api;
        private const string BaseUrl = "https://sc.ftqq.com/";

        private ServerchanRepository()
        {
            _api = RestService.For<IServerchanApi>(BaseUrl);
        }

        /// <summary>
        /// Send a message via Serverchan.
        /// </summary>
        /// <param name="sckey">The key of Serverchan.</param>
        /// <param name="title">The title of message.</param>
        /// <param name="content">The content of message.</param>
        /// <returns>A task that represents the status.</returns>
        public async Task<bool> SendMessage(string sckey, string title, string content)
        {
            var response = await _api.SendMessage(sckey, title, content);

            return response.Code == 0;
        }
    }
}