using System.Threading.Tasks;
using AutoXduNCovReport.Model;
using Refit;

namespace AutoXduNCovReport.Api
{
    /// <summary>
    /// Network API for Serverchan.
    /// </summary>
    internal interface IServerchanApi
    {
        /// <summary>
        /// Send a message via Serverchan.
        /// </summary>
        /// <param name="sckey">The key of Serverchan.</param>
        /// <param name="title">The title of message.</param>
        /// <param name="content">The content of message.</param>
        /// <returns>A task that represents the result.</returns>
        [Get("/{sckey}.send?text={title}&desp={content}")]
        Task<ServerchanResponse> SendMessage(string sckey, string title, string content);
    }
}