using Microsoft.AspNetCore.Identity.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore;
using Rent.Entities.Cities;
using Rent.Entities.Comments;
using Rent.Entities.Photos;
using Rent.Entities.Properties;
using Rent.Entities.Responses;
using Rent.Entities.Users;

namespace Rent.Storage.Configuration
{
    public class ApplicationDbContext : IdentityDbContext<User>
    {
        public DbSet<City> Cities { get; set; }
        public DbSet<Property> Properties { get; set; }
        public DbSet<Photo> Photos { get; set; }
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
                   .HasOne(p => p.Landlord)
                   .WithMany(u => u.Properties)
                   .HasForeignKey(p => p.LandlordId);

                builder
                    .HasOne(p => p.City)
                    .WithMany(c => c.Properties)
                    .HasForeignKey(p => p.CityId)
                    .OnDelete(DeleteBehavior.Restrict);

                builder.Property(c => c.Address).IsRequired().HasMaxLength(100);
                builder.Property(c => c.Description).IsRequired().HasMaxLength(500);
                builder.Property(c => c.Price).IsRequired();
                builder.Property(c => c.Status).IsRequired();
            });

            modelBuilder.Entity<Photo>(builder =>
            {
                builder.Property(p => p.Id).ValueGeneratedNever();

                builder
                    .HasOne(p => p.Property)
                    .WithMany(p => p.Photos)
                    .HasForeignKey(p => p.PropertyId);
            });

            modelBuilder.Entity<Response>(builder =>
            {
                builder
                    .HasOne(p => p.Tenant)
                    .WithMany(u => u.Responses)
                    .HasForeignKey(p => p.TenantId);

                builder
                   .HasOne(p => p.Property)
                   .WithMany(u => u.Responses)
                   .HasForeignKey(p => p.PropertyId);

                builder.Property(c => c.Message).IsRequired().HasMaxLength(400);
                builder.Property(c => c.Status).IsRequired();
            });

            modelBuilder.Entity<Comment>(builder =>
            {
                builder
                   .HasOne(p => p.Tenant)
                   .WithMany(u => u.Comments)
                   .HasForeignKey(p => p.TenantId);

                builder
                    .HasOne(p => p.Property)
                    .WithMany(u => u.Comments)
                    .HasForeignKey(p => p.PropertyId);

                builder.Property(c => c.Message).IsRequired().HasMaxLength(400);
                builder.Property(c => c.Rate).IsRequired();
            });

            base.OnModelCreating(modelBuilder);
        }
    }
}
