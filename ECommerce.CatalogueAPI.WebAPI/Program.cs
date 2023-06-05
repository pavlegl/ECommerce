using System.Reflection.PortableExecutable;

var builder = WebApplication.CreateBuilder(args);

// Add services to the container.

builder.Services.AddControllers();
// Learn more about configuring Swagger/OpenAPI at https://aka.ms/aspnetcore/swashbuckle
builder.Services.AddEndpointsApiExplorer();
builder.Services.AddSwaggerGen();

builder.Services.AddHeaderPropagation(o =>
{
    // propagate the header with same name if exist
    o.Headers.Add("Authorization");
    o.Headers.Add("Accept-Language");    // Propagate header with different name if exist
    o.Headers.Add("Accept-Language");    // Propagate header with different name if exist
    o.Headers.Add("Accept-Language", "Lang");    // Propagate header with same name and a default value
    o.Headers.Add("Accept-Language", context => "en");    // Propagate header with different name and a default value
    o.Headers.Add("Accept-Language", "Lang", context => "en");
});

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
