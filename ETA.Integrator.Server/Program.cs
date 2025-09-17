using ETA.Integrator.Server.Extensions;
using ETA.Integrator.Server.Middlewares;

var builder = WebApplication.CreateBuilder(args);

var corsOrigins = builder.Configuration.GetSection("AllowedCorsOrigins").Get<string[]>();

builder.Services.AddCors(options =>
{
    options.AddPolicy("AllowAll", builder =>
    {
        builder.WithOrigins(corsOrigins ?? [])
               .AllowAnyMethod()
               .AllowAnyHeader()
               .AllowCredentials();
    });
});

builder.Services.AddAllServices(builder.Configuration);

var app = builder.Build();

app.UseHttpsRedirection();
app.UseStaticFiles();
app.UseRouting();
app.UseCors("AllowAll");
//app.UseDefaultFiles();

// Configure the HTTP request pipeline.
if (app.Environment.IsDevelopment())
{
    app.UseSwagger();
    app.UseSwaggerUI();
}

app.UseMiddleware<ExceptionHandlingMiddleware>();

app.UseAuthorization();

app.MapControllers();

app.MapFallbackToFile("index.html");
app.Run();




