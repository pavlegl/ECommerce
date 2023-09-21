using Microsoft.AspNetCore.Builder;
using Microsoft.AspNetCore.Http;
using System.Text;
using System.Threading.Tasks;

namespace ECommerce.IdentityAPI.WebAPI
{
    // You may need to install the Microsoft.AspNetCore.Http.Abstractions package into your project
    public class CustomMiddleware
    {
        private readonly RequestDelegate _next;
        private IFixedGuidProvider _fgpSingleton;

        public CustomMiddleware(RequestDelegate next, IFixedGuidProvider fgpSingleton)
        {
            _next = next;
            _fgpSingleton = fgpSingleton;
        }

        public async Task InvokeAsync(HttpContext httpContext, IFixedGuidProvider fgpScoped)
        {
            var req = httpContext.Request;

            Console.WriteLine("Message from CustomMiddleware | GuidSingleton=" + _fgpSingleton.GetGuid() + " | GuidScoped=" + fgpScoped.GetGuid() + "): " + req.Path.ToString() + " | Method: " + req.Method);
            await _next(httpContext);
        }
    }

    // Extension method used to add the middleware to the HTTP request pipeline.
    public static class CustomMiddlewareExtensions
    {
        public static IApplicationBuilder UseCustomMiddleware(this IApplicationBuilder builder)
        {
            return builder.UseMiddleware<CustomMiddleware>();
        }
    }
}
