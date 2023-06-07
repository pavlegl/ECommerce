using ECommerce;
using ECommerce.CatalogueAPI.BL;
using ECommerce.CatalogueAPI.Common;
using ECommerce.CatalogueAPI.DAL;
using Microsoft.Extensions.Configuration;
using System.Reflection.PortableExecutable;

var builder = WebApplication.CreateBuilder(args);

// Add services to the container.

builder.Services.AddControllers();
// Learn more about configuring Swagger/OpenAPI at https://aka.ms/aspnetcore/swashbuckle
builder.Services.AddEndpointsApiExplorer();
builder.Services.AddSwaggerGen();

var config1 = builder.Configuration["UrlECommerceIdentityApi"];


builder.Services.AddHeaderPropagation(o =>
{
    o.Headers.Add("Authorization");
});

builder.Services.AddSingleton<IConfiguration>(builder.Configuration);
builder.Services.AddScoped<IProductBL, ProductBL>();
builder.Services.AddScoped<IProductDAL, ProductDAL>();
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
