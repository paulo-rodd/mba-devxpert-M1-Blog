using Blog.Application.Interfaces;
using Blog.Domain.Entities;
using Blog.Infra.Data;
using Microsoft.AspNetCore.Identity;
using Microsoft.EntityFrameworkCore;

namespace Blog.Infra.Configurations
{
    public static class DbMigrationHelpersExtension
    {
        public static void UseDbMigrationHelper(this WebApplication app)
        {
            DbMigrationHelpers.EnsureSeedData(app).Wait();
        }
    }

    public class DbMigrationHelpers
    {

        public static async Task EnsureSeedData(WebApplication serviceScope)
        {
            var services = serviceScope.Services.CreateScope().ServiceProvider;
            await EnsureSeedData(services);
        }

        public static async Task EnsureSeedData(IServiceProvider serviceProvider)
        {
            using var scope = serviceProvider.GetRequiredService<IServiceScopeFactory>().CreateScope();
            var env = scope.ServiceProvider.GetRequiredService<IWebHostEnvironment>();

            var contextBlog = scope.ServiceProvider.GetRequiredService<BlogDbContext>();
            var contextId = scope.ServiceProvider.GetRequiredService<IdDbContext>();

            if (env.IsDevelopment())
            {
                await contextBlog.Database.MigrateAsync();
                await contextId.Database.MigrateAsync();

                await EnsureSeedAdmin(serviceProvider);
            }
        }


        public static async Task EnsureSeedAdmin(IServiceProvider serviceProvider)
        {
            using var scope = serviceProvider.GetRequiredService<IServiceScopeFactory>().CreateScope();
            var contextId = scope.ServiceProvider.GetRequiredService<IdDbContext>();
            var config = scope.ServiceProvider.GetRequiredService<IConfiguration>();
            var userManager = scope.ServiceProvider.GetRequiredService<UserManager<IdentityUser>>();

            IdentityResult result;

            if (contextId.Roles.Any()) return;

            var roleManager = scope.ServiceProvider.GetRequiredService<RoleManager<IdentityRole>>();

            var adminRoleName = "Admin";
            var userRoleName = "User";

            var roleExists = await roleManager.RoleExistsAsync(adminRoleName);

            if (!roleExists)
            {
                result = await roleManager.CreateAsync(new IdentityRole(adminRoleName));
                var resultUserRole = await roleManager.CreateAsync(new IdentityRole(userRoleName));

                if (result.Succeeded)
                {
                    var adminEmail = config["AdminCredentials:AdminEmail"]!.ToString();
                    var adminPassword = config["AdminCredentials:AdminPassword"]!.ToString();

                    var admin = await userManager.FindByEmailAsync(adminEmail);

                    if (admin == null)
                    {
                        admin = new IdentityUser
                        {
                            Email = adminEmail,
                            NormalizedEmail = adminEmail.ToUpper(),
                            UserName = adminEmail,
                            NormalizedUserName = adminEmail.ToUpper(),
                            ConcurrencyStamp = Guid.NewGuid().ToString(),
                            LockoutEnabled = false,
                            TwoFactorEnabled = false,
                            AccessFailedCount = 0,
                            EmailConfirmed = true,
                            SecurityStamp = Guid.NewGuid().ToString()
                        };

                        result = await userManager.CreateAsync(admin, adminPassword);

                        if (result.Succeeded)
                        {
                            await userManager.AddToRoleAsync(admin, adminRoleName);

                            await EnsureSeedAutor(serviceProvider, admin);
                        }
                        else
                        {
                            throw new Exception("Falha ao criar o usuário administrador.");
                        }
                    }
                }
            }
        }

        public static async Task EnsureSeedAutor(IServiceProvider serviceProvider, IdentityUser admin)
        {
            using var scope = serviceProvider.GetRequiredService<IServiceScopeFactory>().CreateScope();
            var context = scope.ServiceProvider.GetRequiredService<BlogDbContext>();

            Autor autor = new Autor
            {
                Nome = "Administrador",
                Email = admin.Email!,
                IdUsuario = admin.Id,
                DataCadastro = DateTime.Now,
                Ativo = true
            };

            context.Autores.Add(autor);
            await context.SaveChangesAsync();
        }
    }
}
