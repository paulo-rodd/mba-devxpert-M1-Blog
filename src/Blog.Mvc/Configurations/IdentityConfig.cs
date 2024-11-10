using Blog.Infra.Data;
using Microsoft.AspNetCore.Identity;

namespace Blog.Mvc.Configurations
{
    public static class IdentityConfig
    {
        public static WebApplicationBuilder AddIdentityConfig(this WebApplicationBuilder builder)
        {
            builder.Services.AddDefaultIdentity<IdentityUser>(options => options.SignIn.RequireConfirmedAccount = true)
                .AddRoles<IdentityRole>()
                .AddEntityFrameworkStores<IdDbContext>();

            return builder;
        }
    }
}
