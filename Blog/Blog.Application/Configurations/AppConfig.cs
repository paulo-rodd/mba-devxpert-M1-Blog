namespace Blog.Application.Configurations
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
