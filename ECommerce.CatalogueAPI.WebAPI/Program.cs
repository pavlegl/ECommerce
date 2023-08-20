using ECommerce;
using ECommerce.CatalogueAPI.BL;
using ECommerce.CatalogueAPI.Common;
using ECommerce.CatalogueAPI.DAL;
using ECommerce.CatalogueAPI.DAL.Models;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Configuration;

var builder = WebApplication.CreateBuilder(args);

// Add services to the container.

builder.Services.AddControllers();
// Learn more about configuring Swagger/OpenAPI at https://aka.ms/aspnetcore/swashbuckle
builder.Services.AddEndpointsApiExplorer();
builder.Services.AddSwaggerGen();

var config = builder.Configuration;
builder.Services.Configure<MainOptions>(MainOptions.OptionsName, config);
MainOptions mainOptions = builder.Configuration.Get<MainOptions>();

builder.Services.AddHeaderPropagation(o =>
{
    o.Headers.Add("Authorization");
});
//builder.Services.AddHttpClient<MyClient>().AddHeaderPropagation();

builder.Services.AddDbContext<EcommerceContext>(options => options.UseSqlServer(mainOptions.ConnectionStrings.ECommerceConnString)); // config.GetConnectionString("ECommerceConnString"));

builder.Services.AddSingleton<IConfiguration>(config);
builder.Services.AddScoped<IProductDAL>(x => new ProductDAL(x.GetRequiredService<EcommerceContext>()));
builder.Services.AddScoped<IProductBL>(x => new ProductBL(x.GetRequiredService<IProductDAL>(), mainOptions.CurrentRegionAlpha3Code));
builder.Services.AddSingleton<IECLogger, ECLogger>();
builder.Services.AddSingleton<BaseECExceptionHandler>(x => new ECExceptionHttpResponseHandler(x.GetRequiredService<IECLogger>()));
var app = builder.Build();

// Configure the HTTP request pipeline.
if (app.Environment.IsDevelopment())
{
    app.UseSwagger();
    app.UseSwaggerUI();
}

app.UseHeaderPropagation();

app.UseHttpsRedirection();

app.UseAuthorization();

app.MapControllers();

/*app.MapWhen(l => l.Request.Query.ContainsKey("hacktheplanet"),
        p => p.Use(async ctx => await ctx.Response.WriteAsync("My IP  Is :" + ctx.Request.HttpContext.Connection.RemoteIpAddress)));
*/

app.MapWhen(
        c => c.Request.Query.ContainsKey("hacktheplanet"), a => a.Run(async context =>
        {
            context.Response.Redirect("https://www.imdb.com/title/tt0113243/?ref_=nv_sr_srsg_0_tt_8_nm_0_q_hackers");
        }));

app.Use(async (context, next) =>
{
    Console.WriteLine("Hello from CatalogueAPI custom delegate, before next.Invoke().");
    await next.Invoke();
    Console.WriteLine("Hello from CatalogueAPI custom delegate, after next.Invoke().");
});

app.Run();
