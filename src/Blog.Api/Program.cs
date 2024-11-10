using Blog.Api.Configurations;
using Blog.Infra.Configurations;

var builder = WebApplication.CreateBuilder(args);

builder
    .AddApiConfig()
    .AddDatabaseConfig()
    .AddIdentityConfig();    


var app = builder.Build();

app.AddApiAppConfig();

app.Run();
