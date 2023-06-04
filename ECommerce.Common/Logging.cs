using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using System.Text;

namespace ECommerce
{

    public class ECLogger : IECLogger
    {
        private static object lockAccessToFile = new Object();

        public void w2l(string message, EnumTypeOfLog logType, Exception excOrig, HttpRequest request)
        {
            lock (lockAccessToFile)
            {
                try
                {
                    string ipAddr = "";
                    try
                    {
                        ipAddr = request?.HttpContext.Connection.RemoteIpAddress?.ToString();
                    }
                    catch (Exception ex1)
                    {
                        ipAddr = "<unavailable IP address, error: " + Common.getWholeException(ex1) + ">";
                    }
                    string msgFinal = "";
                    if (logType == EnumTypeOfLog.Error)
                        msgFinal += "Error: ";
                    msgFinal += "[" + ipAddr + "] " + message;

                    if (excOrig != null)
                        msgFinal += " Exception: " + Common.getWholeException(excOrig);

                    using (StreamWriter sw = new StreamWriter(Common.getCurrentDir() + "\\ECommerce_Log.txt", true, Encoding.UTF8))
                    {
                        sw.WriteLine(DateTime.Now.ToString("dd.MM.yyyy HH:mm:ss") + " | " + msgFinal);
                    }
                }
                catch (Exception)
                {
                }
            }
        }

    }
}