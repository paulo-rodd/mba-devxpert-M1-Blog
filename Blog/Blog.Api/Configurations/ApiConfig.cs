using Blog.Infra.Configurations;
using Blog.Infra.Data;
using Microsoft.EntityFrameworkCore;
using Microsoft.OpenApi.Models;

namespace Blog.Api.Configurations
{
    public static class ApiConfig
    {
        public static void GetDbAppContext(this WebApplication app)
        {
            using var scope = app.Services.CreateScope();

            scope.ServiceProvider.GetRequiredService<BlogDbContext>().Database.Migrate();
            scope.ServiceProvider.GetRequiredService<IdDbContext>().Database.Migrate();
        }

        public static WebApplication AddApiAppConfig(this WebApplication app)
        {
            app.GetDbAppContext();

            if (app.Environment.IsDevelopment())
            {
                app.UseSwagger();
                app.UseSwaggerUI();
            }

            app.UseHttpsRedirection();

            app.UseAuthentication();
            app.UseAuthorization();

            app.MapControllers();

            app.UseDbMigrationHelper();

            return app;
        }

        public static WebApplicationBuilder AddApiConfig(this WebApplicationBuilder builder)
        {
            builder.Services.AddControllers()
                .ConfigureApiBehaviorOptions(options =>
                {
                    options.SuppressModelStateInvalidFilter = true;
                });

            builder.Services.AddSwaggerGen(c =>
            {
                c.AddSecurityDefinition("Bearer", new OpenApiSecurityScheme
                {
                    Description = "Insira um token JWT desta maneira: Bearer {seu token}.",
                    Name= "Authorization",
                    Scheme = "Bearer",
                    BearerFormat = "JWT",
                    In = ParameterLocation.Header,
                    Type = SecuritySchemeType.ApiKey
                });

                c.AddSecurityRequirement(new OpenApiSecurityRequirement
                {
                    {
                        new OpenApiSecurityScheme
                        {
                            Reference = new OpenApiReference
                            {
                                Type = ReferenceType.SecurityScheme,
                                Id = "Bearer"
                            }
                        },
                        new string[] {}
                    }
                });
            });


            return builder;
        }
    }
}
