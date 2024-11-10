using Blog.Infra.Configurations;

var builder = WebApplication.CreateBuilder(args);

builder.AddDatabaseConfig();


var app = builder.Build();

app.AddAppConfig();

app.Run();
