#nullable disable
using ECommerce;
using ECommerce.IdentityAPI.BL;
using ECommerce.IdentityAPI.Common;
using ECommerce.IdentityAPI.DAL;
using ECommerce.IdentityAPI.DAL.Models;
using ECommerce.IdentityAPI.WebAPI;
using Microsoft.AspNetCore.Authentication.JwtBearer;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Configuration;
using Microsoft.IdentityModel.Tokens;
using System.Text.Json.Serialization;

var builder = WebApplication.CreateBuilder(args);

builder.Services.AddControllers().AddJsonOptions(options => options.JsonSerializerOptions.ReferenceHandler = ReferenceHandler.Preserve);
// Learn more about configuring Swagger/OpenAPI at https://aka.ms/aspnetcore/swashbuckle
builder.Services.AddEndpointsApiExplorer();
builder.Services.AddSwaggerGen();

var config = builder.Configuration;

builder.Services.Configure<MainOptions>(MainOptions.OptionsName, config);
MainOptions mainOptions = builder.Configuration.Get<MainOptions>();
builder.Services.AddDbContext<EcommerceContext>(options => options.UseSqlServer(mainOptions.ConnectionStrings.ECommerceConnString));

string sJwtKeyBase64 = mainOptions.JwtSettings.SecretKey;
byte[] arJwtKey = Convert.FromBase64String(sJwtKeyBase64);

builder.Services.AddAuthentication(l =>
{
    l.DefaultAuthenticateScheme = JwtBearerDefaults.AuthenticationScheme;
    l.DefaultChallengeScheme = JwtBearerDefaults.AuthenticationScheme;
    l.DefaultScheme = JwtBearerDefaults.AuthenticationScheme;
}).AddJwtBearer(l =>
{
    l.TokenValidationParameters = new Microsoft.IdentityModel.Tokens.TokenValidationParameters
    {
        IssuerSigningKey = new SymmetricSecurityKey(arJwtKey),
        ValidateIssuer = false,
        ValidateAudience = false,
        ValidateLifetime = true,
        ValidateIssuerSigningKey = false
    };
});

builder.Services.AddAuthorization(l =>
{
    l.AddPolicy(CustomAuthPolicies.IsAdmin, o => o.RequireClaim(CustomClaims.IsAdmin, "true"));
    l.AddPolicy(CustomAuthPolicies.IsCustomer, o => o.RequireClaim(CustomClaims.IsCustomer, "true"));
});


builder.Services.AddHeaderPropagation(o =>
{
    o.Headers.Add("Authorization");
});
builder.Services.AddHttpContextAccessor();

//builder.Services.AddSingleton<IConfiguration>(config);
builder.Services.AddScoped<IUserDAL, UserDAL>();//(l => new UserDAL(mainOptions, l.GetRequiredService<EcommerceContext>()));
builder.Services.AddScoped<IUserBL, UserBL>();
builder.Services.AddSingleton<IECLogger, ECLogger>();
builder.Services.AddSingleton<BaseECExceptionHandler>(l => new ECExceptionHttpResponseHandler(l.GetRequiredService<IECLogger>()));
builder.Services.AddScoped<IECAuthContainerModel>(l => new JwtContainerModel
{
    SecurityAlgorithm = SecurityAlgorithms.HmacSha256Signature,
    SecretKeyBase64 = sJwtKeyBase64
});
builder.Services.AddScoped<IECAuthService>(l => new JwtService(l.GetRequiredService<IECAuthContainerModel>()));

var app = builder.Build();

// Configure the HTTP request pipeline.
if (app.Environment.IsDevelopment())
{
    app.UseSwagger();
    app.UseSwaggerUI();
}
app.UseMiddleware<CustomMiddleware>();
app.UseExceptionHandler("/api/ExcHandler/HandleError");
app.UseHttpsRedirection();

app.UseAuthentication();
app.UseAuthorization();


app.MapControllers();

app.Run();
