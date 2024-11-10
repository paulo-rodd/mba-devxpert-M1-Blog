using Microsoft.AspNetCore.Identity.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore;

namespace Blog.Infra.Data
{
    public class IdDbContext : IdentityDbContext
    {
        public IdDbContext(DbContextOptions<IdDbContext> options) : base(options)
        {
        }
    }
}
