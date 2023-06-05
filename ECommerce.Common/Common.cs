#nullable disable
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using System.Text;
using System.Security.Cryptography;
using Newtonsoft.Json;
using AutoMapper;

namespace ECommerce
{
    public class Common
    {

        public static string getCurrentDir()
        {
            return System.AppDomain.CurrentDomain.BaseDirectory;
        }

        public static string getWholeException(Exception exc)
        {
            if (exc == null)
                return "";
            StringBuilder sb = new StringBuilder();
            sb.Append(exc.Message);
            while (exc.InnerException != null)
            {
                sb.Append(" - " + exc.InnerException.Message);
                exc = exc.InnerException;
            }
            return sb.ToString();
        }

        public static string hashSha256(string input)
        {
            string sSalt = "_!aF89qz#/+pP0ryu^";
            SHA256CryptoServiceProvider provider = new SHA256CryptoServiceProvider();
            byte[] data = provider.ComputeHash(Encoding.UTF8.GetBytes(input + sSalt));
            StringBuilder sBuilder = new StringBuilder();
            for (int i = 0; i < data.Length; i++)
                sBuilder.Append(data[i].ToString("x2"));
            return sBuilder.ToString();
        }

        public static TDestination Map<TSourceBaseType, TDestinationBaseType, TSource, TDestination>(TSource source)
        {
            var config = new MapperConfiguration(cfg =>
            {
                cfg.CreateMap<TSourceBaseType, TDestinationBaseType>();

            });
            IMapper iMapper = config.CreateMapper();
            return iMapper.Map<TSource, TDestination>(source);
        }

        public static string jsonSerializeIgnoreNulls(object obj)
        {
            if (obj == null)
                return String.Empty;
            return JsonConvert.SerializeObject(obj, new JsonSerializerSettings { NullValueHandling = NullValueHandling.Ignore });
        }

    }

    public interface IECConfig
    {
        BaseECExceptionHandler ExceptionHandler { get; }
        IECLogger Logger { get; }
    }

    public class ECConfig : IECConfig
    {
        BaseECExceptionHandler _exceptionHandler;
        IECLogger _logger;

        public BaseECExceptionHandler ExceptionHandler { get { return _exceptionHandler; } }
        public IECLogger Logger { get { return _logger; } }

        public ECConfig(BaseECExceptionHandler exceptionHandler, IECLogger logger)
        {
            _exceptionHandler = exceptionHandler;
            _logger = logger;
        }

    }

}