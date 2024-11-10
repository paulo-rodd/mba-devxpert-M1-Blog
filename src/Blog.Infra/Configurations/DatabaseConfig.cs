using Blog.Application.Interfaces;
using Blog.Application.Services;
using Blog.Domain.Interfaces;
using Blog.Infra.Data;
using Blog.Infra.Repositories;
using Microsoft.EntityFrameworkCore;


namespace Blog.Infra.Configurations
{
    public static class DatabaseConfig
    {
        public static WebApplicationBuilder AddDatabaseConfig(this WebApplicationBuilder builder)
        {
            if (!builder.Environment.IsProduction())
            {
                builder.Services.AddDbContext<BlogDbContext>(options =>
                    options.UseSqlite(builder.Configuration.GetConnectionString("SQLiteBlogConnection")));

                builder.Services.AddDbContext<IdDbContext>(options =>
                    options.UseSqlite(builder.Configuration.GetConnectionString("SQLiteIdConnection")));
            }
            else
            {
                builder.Services.AddDbContext<BlogDbContext>(options =>
                    options.UseSqlServer(builder.Configuration.GetConnectionString("SQLServerBlogConnection")));

                builder.Services.AddDbContext<IdDbContext>(options =>
                    options.UseSqlServer(builder.Configuration.GetConnectionString("SQLServerIdConnection")));
            }

            builder.Services.AddScoped<IAutorService, AutorService>();
            builder.Services.AddScoped<IAutorRepository, AutorRepository>();
            builder.Services.AddScoped<IPostagemService, PostagemService>();
            builder.Services.AddScoped<IPostagemRepository, PostagemRepository>();
            builder.Services.AddScoped<IComentarioService, ComentarioService>();
            builder.Services.AddScoped<IComentarioRepository, ComentarioRepository>();

            builder.Services.AddDatabaseDeveloperPageExceptionFilter();

            return builder;
        }
    }
}
