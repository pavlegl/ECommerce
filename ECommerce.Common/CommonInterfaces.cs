using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using System.Text;

namespace ECommerce
{
    /// <summary>
    /// Logger of messages.
    /// </summary>
    public interface IECLogger
    {
        /// <summary>
        /// Logs a message.
        /// </summary>
        /// <param name="message">Message to log.</param>
        /// <param name="logType">Type of message.</param>
        /// <param name="excOrig">Optional original Exception.</param>
        /// <param name="request">Optional HttpRequest.</param>
        void w2l(string message, EnumTypeOfLog logType, Exception excOrig, HttpRequest request);
    }

    /// <summary>
    /// Handles REST API exceptions.
    /// </summary>
    public abstract class BaseECExceptionHandler
    {
        public BaseECExceptionHandler(IECLogger logger)
        {
            if (logger == null)
                throw new Exception("Parameter logger must be provided.");
        }

        public abstract ObjectResult ReturnHttpResponse(Exception ex, HttpRequest request);
    }

    public enum EnumTypeOfLog
    {
        Information,
        Error
    }

}