using Microsoft.EntityFrameworkCore;
using Blog.Infra.Data;
using Blog.Infra.Configurations;
namespace Blog.Mvc.Configurations
{
    public static class AppConfig
    {
        public static void GetDbAppContext(this WebApplication app)
        {
            using var scope = app.Services.CreateScope();

            scope.ServiceProvider.GetRequiredService<BlogDbContext>().Database.Migrate();
            scope.ServiceProvider.GetRequiredService<IdDbContext>().Database.Migrate();
        }

        public static WebApplicationBuilder AddAppBuilderConfig(this WebApplicationBuilder builder)
        {
            builder.Services.AddControllersWithViews();

            return builder;
        }


        public static WebApplication AddAppMvcConfig(this WebApplication app)
        {
            app.GetDbAppContext();

            if (app.Environment.IsDevelopment())
            {
                app.UseMigrationsEndPoint();
            }
            else
            {
                app.UseExceptionHandler("/Home/Error");
                app.UseHsts();
            }


            app.UseHttpsRedirection();
            app.UseStaticFiles();

            app.UseRouting();

            app.UseAuthentication();
            app.UseAuthorization();

            app.MapControllerRoute(
                name: "default",
                pattern: "{controller=Home}/{action=Index}/{id?}");
            app.MapRazorPages();

            app.UseDbMigrationHelper();

            return app;
        }
    }
}
