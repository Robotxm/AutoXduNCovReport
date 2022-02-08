using System;
using System.Threading.Tasks;
using AutoXduNCovReport.Api;
using Refit;

namespace AutoXduNCovReport.Repository
{
    internal class PushPlusRepository
    {
        private static readonly Lazy<PushPlusRepository> Lazy = new(() => new PushPlusRepository());

        /// <summary>
        /// Get the singleton of the repository.
        /// </summary>
        public static PushPlusRepository Instance => Lazy.Value;

        private readonly IPushPlus _api;
        private const string BaseUrl = "https://www.pushplus.plus/";

        private PushPlusRepository()
        {
            _api = RestService.For<IPushPlus>(BaseUrl);
        }

        /// <summary>
        /// Send a message via Serverchan.
        /// </summary>
        /// <param name="token">The key of Serverchan.</param>
        /// <param name="title">The title of message.</param>
        /// <param name="content">The content of message.</param>
        /// <returns>A task that represents the status.</returns>
        public async Task<bool> SendMessage(string token, string title, string content)
        {
            var response = await _api.SendMessage(token, title, content);

            return response.Code == 0;
        }
    }
}
