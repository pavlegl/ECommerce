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

app.Run();
