using ECommerce.CatalogueAPI.Common;
using ECommerce.CatalogueAPI.DAL.Models;
using Microsoft.AspNetCore.Builder;
using Microsoft.AspNetCore.Hosting;
using Microsoft.AspNetCore.Mvc.Testing;
using Microsoft.Extensions.Configuration;
using Microsoft.EntityFrameworkCore;

namespace ECommerce.CatalogueAPI.IntegrationTesting
{
    public class CustomWebApplicationFactory<TProgram> : WebApplicationFactory<TProgram> where TProgram : class
    {
        protected override void ConfigureWebHost(IWebHostBuilder builder)
        {
            /*builder.ConfigureServices(services =>
            {
                services.AddControllers();
                // Learn more about configuring Swagger/OpenAPI at https://aka.ms/aspnetcore/swashbuckle
                services.AddEndpointsApiExplorer();
                services.AddSwaggerGen();

                //var config1 = builder.Configuration["UrlECommerceIdentityApi"];
                var config = builder.Configuration;

                services.AddHeaderPropagation(o =>
                {
                    o.Headers.Add("Authorization");
                });
                //services.AddHttpClient<MyClient>().AddHeaderPropagation();

                services.AddDbContext<EcommerceContext>(options => options.UseSqlServer(config.GetConnectionString("ECommerceConnString")));

                services.AddSingleton<IConfiguration>(config);
                //services.AddScoped<DbContext, EcommerceContext>();
                services.AddScoped<IProductDAL>(x => new ProductDAL(x.GetRequiredService<EcommerceContext>()));
                services.AddScoped<IProductBL>(x => new ProductBL(x.GetRequiredService<IProductDAL>(), "GER"));
                //services.AddScoped<IProductDAL>(l => new ProductDAL(new EcommerceContext()));
                services.AddSingleton<IECLogger, ECLogger>();
                services.AddSingleton<BaseECExceptionHandler>(x => new ECExceptionHttpResponseHandler(x.GetRequiredService<IECLogger>()));
            });*/

            builder.UseEnvironment("Development");
        }
    }
}
