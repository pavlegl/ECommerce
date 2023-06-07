#nullable disable
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;

namespace ECommerce
{
    public class ECException : Exception
    {
        private ObjectResult _httpResponseObject = null;
        public ObjectResult HttpResponseObject { get { return _httpResponseObject; } }
        private int _statusCode;
        public int StatusCode { get { return _statusCode; } }
        /*private EnumExceptionType _exceptionType = EnumExceptionType.Other;
        public EnumExceptionType ExceptionType { get { return _exceptionType; } }*/

        public ECException(string message) : base(message) { }

        public ECException(ObjectResult httpResponseCode) : base("HTTP response status code=" + httpResponseCode?.StatusCode + "; Value=" + httpResponseCode?.Value?.ToString() + ".")
        {
            _httpResponseObject = httpResponseCode;
        }

        public ECException(int statusCode, string message) : base(message)
        {
            _statusCode = statusCode;
            _httpResponseObject = new ObjectResult(message) { StatusCode = statusCode };
        }

        public ECException(int statusCode) : base("HTTP response status code=" + statusCode.ToString() + ".")
        {
            _statusCode = statusCode;
            _httpResponseObject = new ObjectResult(null) { StatusCode = statusCode };
        }

        /*public ECException(EnumExceptionType exceptionType, int statusCode, string message) : base(articulateException(exceptionType))
        {
            _httpResponseObject = new ObjectResult(message) { StatusCode = statusCode };
            _exceptionType = exceptionType;
        }

        private static string articulateException(EnumExceptionType eet)
        {
            switch (eet)
            {
                case EnumExceptionType.EmailBadFormat:
                    return "Bad format of the email address.";
                case EnumExceptionType.UserNameNotProperlyFormated:
                    return "Username doesn't meet format requirements.";
                case EnumExceptionType.CountryNotSupported:
                    return "The country is not supported.";
                case EnumExceptionType.UserMustProvidePostCode:
                    return "User from this country must provide postcode.";
                case EnumExceptionType.EmailAlreadyUsed:
                    return "Provided email address is already used by another user.";
                case EnumExceptionType.Other:
                    return "Unknown error.";
                default:
                    return "Unknown error.";
            }
        }*/

    }



    public class ECExceptionHttpResponseHandler : BaseECExceptionHandler
    {
        IECLogger _logger;

        /// <summary>
        /// Constructor for the class.
        /// </summary>
        /// <param name="logger">Optional IECLogger object for logging the Exception.</param>
        public ECExceptionHttpResponseHandler(IECLogger logger) : base(logger)
        {
            _logger = logger;
        }

        /// <summary>
        /// Logs the Exception and returns the appropriate HttpResponseCode. With the HttpRequest param
        /// it is able to log the IP address or IdUser of the caller.
        /// </summary>
        /// <param name="ex">Exception that is handled.</param>
        /// <param name="request">Optional HttpRequest for registering the IP address.</param>
        /// <returns>If a regular Exception, returns: 500 + Message.</returns>
        public override ObjectResult ReturnHttpResponse(Exception ex, HttpRequest request)
        {
            _logger?.w2l(ex.Message, EnumTypeOfLog.Error, ex, request);

            ObjectResult orResp = new ObjectResult(EcCommon.getWholeException(ex))
            {
                StatusCode = StatusCodes.Status500InternalServerError // <--- Default value.
            };

            if (ex == null)
                return orResp;

            if (ex is ECException)
            {
                int? statusCode = ((ECException)ex).HttpResponseObject?.StatusCode;
                if (statusCode.HasValue)
                    orResp.StatusCode = statusCode;
            }

            return orResp;
        }
    }

    /*public enum EnumExceptionType
    {
        EmailBadFormat,
        EmailAlreadyUsed,
        CountryNotSupported,
        UserNameNotProperlyFormated,
        PasswordNotProperlyFormated,
        UserMustProvidePostCode,
        Other
    }*/
}