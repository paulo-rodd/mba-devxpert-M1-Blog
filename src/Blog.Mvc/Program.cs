using Blog.Infra.Configurations;
using Blog.Mvc.Configurations;
using Blog.Infra.Data;
using Microsoft.AspNetCore.Identity;
using Microsoft.EntityFrameworkCore;

var builder = WebApplication.CreateBuilder(args);

builder
    .AddDatabaseConfig()
    .AddIdentityConfig()
    .AddAppBuilderConfig();

var app = builder.Build();

app.AddAppMvcConfig();

app.Run();
