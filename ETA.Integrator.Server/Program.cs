using DotNetEnv;
using ETA.Integrator.Server.Interface;
using ETA.Integrator.Server.Services;

var builder = WebApplication.CreateBuilder(args);

// Add services to the container.

builder.Services.AddControllers();
// Learn more about configuring Swagger/OpenAPI at https://aka.ms/aspnetcore/swashbuckle
builder.Services.AddEndpointsApiExplorer();
builder.Services.AddSwaggerGen();
builder.Services.AddHttpClient();

builder.Services.AddTransient<IConfigurationService, ConfigurationService>();

var app = builder.Build();

app.UseDefaultFiles();
app.UseStaticFiles();

// Configure the HTTP request pipeline.
if (app.Environment.IsDevelopment())
{
    app.UseSwagger();
    app.UseSwaggerUI();
}

app.UseHttpsRedirection();

app.UseAuthorization();

app.MapControllers();

app.MapFallbackToFile("/index.html");

Env.Load();

//var HMS_API = Environment.GetEnvironmentVariable("HMS_API");

//app.MapGet("/config/landing", () =>
//{
//    var landing = HMS_API == null ? "settings" : "login";
//    return Results.Json(new { landing });
//});

app.Run();




