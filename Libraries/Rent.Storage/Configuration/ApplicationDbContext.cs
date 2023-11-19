using Microsoft.AspNetCore.Identity.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore;
using Rent.Entities.Cities;
using Rent.Entities.Comments;
using Rent.Entities.Properties;
using Rent.Entities.Responses;
using Rent.Entities.Users;
using System.Reflection.Emit;

namespace Rent.Storage.Configuration
{
    public class ApplicationDbContext : IdentityDbContext<User>
    {
        public DbSet<City> Cities { get; set; }
        public DbSet<Property> Properties { get; set; }
        public DbSet<Response> Responses { get; set; }
        public DbSet<Comment> Comments { get; set; }


        public ApplicationDbContext(DbContextOptions<ApplicationDbContext> options) : base(options)
        {
        }

        protected override void OnModelCreating(ModelBuilder modelBuilder)
        {
            modelBuilder.Entity<City>(builder =>
            {
                builder.Property(c => c.Name).IsRequired().HasMaxLength(30);
            });

            modelBuilder.Entity<Property>(builder =>
            {
                builder
                    .HasOne(p => p.City)
                    .WithMany(c => c.Properties)
                    .HasForeignKey(p => p.CityId)
                    .OnDelete(DeleteBehavior.SetNull);

                builder.Property(c => c.Address).IsRequired().HasMaxLength(100);
                builder.Property(c => c.Description).IsRequired().HasMaxLength(500);
                builder.Property(c => c.Price).IsRequired();
                builder.Property(c => c.Status).IsRequired();
            });

            modelBuilder.Entity<Response>(builder =>
            {
                builder.Property(c => c.Message).IsRequired().HasMaxLength(400);
                builder.Property(c => c.Status).IsRequired();
            });

            modelBuilder.Entity<Comment>(builder =>
            {
                builder.Property(c => c.Message).IsRequired().HasMaxLength(400);
                builder.Property(c => c.Rate).IsRequired();
            });

            base.OnModelCreating(modelBuilder);
        }
    }
}
