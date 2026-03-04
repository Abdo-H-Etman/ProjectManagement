using API.Extensions;
using Application.Services;
using DotNetEnv;
using Serilog;

Env.Load();

var builder = WebApplication.CreateBuilder(args);

builder.Services.ConfigureAllServices(builder.Configuration);
builder.Services.AddOpenApi();
builder.Services.AddControllers();
builder.Services.AddAutoMapper( assemblies => 
{
    assemblies.AddMaps(typeof(TaskService).Assembly);
    assemblies.AddMaps(typeof(Program).Assembly);
});
Log.Logger = new LoggerConfiguration()
    .ReadFrom.Configuration(builder.Configuration)
    .CreateLogger();
var app = builder.Build();

// Configure the HTTP request pipeline.
if (app.Environment.IsDevelopment())
{
    app.MapOpenApi();
}

app.UseHttpsRedirection();
app.UseCors("CorsPolicy");
app.UseAuthentication();
app.UseAuthorization();

app.MapControllers();


app.Run();
