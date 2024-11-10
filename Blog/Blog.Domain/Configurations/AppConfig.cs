namespace Blog.Domain.Configurations
{
    public static class AppConfig
    {
        public static WebApplication AddAppConfig(this WebApplication app)
        {
            app.MapGet("/", () => "Hello World!");

            return app;
        }
    }
}
