using Blog.Domain.Configurations;

var builder = WebApplication.CreateBuilder(args);
var app = builder.Build();

app.AddAppConfig();

app.Run();
