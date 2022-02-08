using System.Threading.Tasks;
using AutoXduNCovReport.Model;
using Refit;

namespace AutoXduNCovReport.Api
{
    /// <summary>
    /// Network API for PushPlus.
    /// </summary>
    internal interface IPushPlus
    {
        /// <summary>
        /// Send a message via PushPlus.
        /// </summary>
        /// <param name="token">The token of PushPlus.</param>
        /// <param name="title">The title of message.</param>
        /// <param name="content">The content of message.</param>
        /// <returns>A task that represents the result.</returns>
        [Get("/send/{token}?title={title}&content={content}")]
        Task<PushPlusResponse> SendMessage(string token, string title, string content);
    }
}
