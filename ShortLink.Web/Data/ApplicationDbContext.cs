using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Identity.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore;
using ShortLink.Web.Data.Entities;

namespace ShortLink.Web.Data
{
    public class ApplicationDbContext : IdentityDbContext<User, Role, int>
    {
        public DbSet<Link> Links { get; set; }

        public ApplicationDbContext(DbContextOptions<ApplicationDbContext> options)
            : base(options)
        {
            Database.EnsureCreated();
        }

        protected override void OnModelCreating(ModelBuilder modelBuilder)
        {
            base.OnModelCreating(modelBuilder);

            modelBuilder.Entity<User>().ToTable(nameof(Users));
            modelBuilder.Entity<Role>().ToTable(nameof(Roles));

            modelBuilder.Ignore(typeof(IdentityUserClaim<>));
            modelBuilder.Ignore(typeof(IdentityRoleClaim<>));
            modelBuilder.Ignore(typeof(IdentityUserLogin<>));
            modelBuilder.Ignore(typeof(IdentityUserToken<>));
        }

    }
}
