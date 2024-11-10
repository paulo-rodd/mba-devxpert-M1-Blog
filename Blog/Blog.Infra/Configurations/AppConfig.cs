namespace Blog.Infra.Configurations
{
    public static class AppConfig
    {
        public static WebApplication AddAppConfig(this WebApplication app)
        {
            app.MapGet("/", () => "Hello World!");

            app.UseDbMigrationHelper();

            return app;
        }
    }
}
    