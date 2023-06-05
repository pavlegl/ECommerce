using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using System.Collections;
using System.Text;

namespace ECommerce
{
    public class ECExceptionHttpResponse : Exception
    {
        private ObjectResult _httpResponseObject = null; // new OkObjectResult(null);
        public ObjectResult HttpResponseObject { get { return _httpResponseObject; } }

        public ECExceptionHttpResponse(string message) : base(message) { }

        public ECExceptionHttpResponse(ObjectResult httpResponseCode) : base("HTTP response status code=" + httpResponseCode?.StatusCode + "; Value=" + httpResponseCode?.Value?.ToString() + ".")
        {
            _httpResponseObject = httpResponseCode;
        }

        public ECExceptionHttpResponse(int statusCode, string message) : base(message)
        {
            _httpResponseObject = new ObjectResult(message) { StatusCode = statusCode };
        }
    }



    public class ECExceptionHttpResponseHandler : BaseECExceptionHandler
    {
        IECLogger _logger;

        /// <summary>
        /// 
        /// </summary>
        /// <param name="logger">Optional IECLogger object for logging the Exception.</param>
        public ECExceptionHttpResponseHandler(IECLogger logger) : base(logger)
        {
            _logger = logger;
            if (_logger == null)
                _logger = new ECLogger();
        }

        /// <summary>
        /// Logs the Exception and returns the appropriate HttpResponseCode.
        /// </summary>
        /// <param name="ex">Exception that is handled.</param>
        /// <param name="request">Optional HttpRequest for registering the IP address.</param>
        /// <returns>If a regular Exception, returns: 500 + Message.</returns>
        public override ObjectResult ReturnHttpResponse(Exception ex, HttpRequest request)
        {
            _logger?.w2l(ex.Message, EnumTypeOfLog.Error, ex, request);

            ObjectResult orResp = new ObjectResult(Common.getWholeException(ex))
            {
                StatusCode = StatusCodes.Status500InternalServerError // <--- Default value.
            };

            if (ex == null)
                return orResp;

            if (ex is ECExceptionHttpResponse)
            {
                int? statusCode = ((ECExceptionHttpResponse)ex).HttpResponseObject?.StatusCode;
                if (statusCode.HasValue)
                    orResp.StatusCode = statusCode;
            }

            return orResp;
        }
    }

    public enum EnumExceptionType
    {
        EmailBadFormat,
        EmailAlreadyUsed,
        CountryNotSupported,
        UserNameNotProperlyFormated,
        PasswordNotProperlyFormated,
        UserMustProvidePostCode,
        Other
    }

    public class ECException : Exception
    {
        private EnumExceptionType _exceptionType = EnumExceptionType.Other;
        public EnumExceptionType ExceptionType { get { return _exceptionType; } }

        public ECException(string message) : base(message) { }

        public ECException(EnumExceptionType exceptionType) : base(articulateException(exceptionType))
        {
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
        }
    }

}