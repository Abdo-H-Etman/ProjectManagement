using API.Extensions;
using DotNetEnv;
using Infrastructure.Utilities;

// Load environment variables from .env before creating the WebApplicationBuilder
// so they become part of the application's configuration sources.
Env.Load();

var builder = WebApplication.CreateBuilder(args);

builder.Services.ConfigureAllServices(builder.Configuration);
builder.Services.AddOpenApi();
builder.Services.AddControllers();
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
