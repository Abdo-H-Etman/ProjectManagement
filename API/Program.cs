using API.Extensions;
using DotNetEnv;

var builder = WebApplication.CreateBuilder(args);

Env.Load();


builder.Services.ConfigureDatabase(builder.Configuration);
builder.Services.AddOpenApi();

var app = builder.Build();

// Configure the HTTP request pipeline.
if (app.Environment.IsDevelopment())
{
    app.MapOpenApi();
}

app.UseHttpsRedirection();


app.Run();
