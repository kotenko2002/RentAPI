using AutoMapper;
using Microsoft.AspNetCore.Identity;
using Microsoft.EntityFrameworkCore;
using Rent.Entities.Cities;
using Rent.Entities.Comments;
using Rent.Entities.Photos;
using Rent.Entities.Properties;
using Rent.Entities.Responses;
using Rent.Entities.Users;
using Rent.Storage.Configuration;
using RentAPI.Infrastructure.Mapper;

namespace Rent.Tests
{
    public static class UnitTestHelper
    {
        public static DbContextOptions<ApplicationDbContext> GetUnitTestDbOptions()
        {
            var options = new DbContextOptionsBuilder<ApplicationDbContext>()
            .UseInMemoryDatabase(Guid.NewGuid().ToString())
            .Options;

            using (var context = new ApplicationDbContext(options))
            {
                SeedData(context);
            }

            return options;
        }

        public static IMapper CreateMapperProfile()
        {
            var myProfile = new AutomapperProfile();
            var configuration = new MapperConfiguration(cfg => cfg.AddProfile(myProfile));

            return new Mapper(configuration);
        }

        public static void SeedData(ApplicationDbContext context)
        {
            context.Cities.AddRange(
                new City { Id = 1, Name = "City1" },
                new City { Id = 2, Name = "City2" }
            );
            context.Users.AddRange(
                new User { Id = "1", UserName = "Landlord1", RefreshToken = "RefreshToken1", RefreshTokenExpiryTime = DateTime.Now },
                new User { Id = "2", UserName = "Landlord2", RefreshToken = "RefreshToken2", RefreshTokenExpiryTime = DateTime.Now },
                new User { Id = "3", UserName = "Tenant1", RefreshToken = "RefreshToken3", RefreshTokenExpiryTime = DateTime.Now }
            );
            context.Roles.AddRange(
                new IdentityRole { Id = "1", Name = Roles.Landlord },
                new IdentityRole { Id = "2", Name = Roles.Landlord },
                new IdentityRole { Id = "3", Name = Roles.Tenant }
            );
            context.UserRoles.AddRange(
                new IdentityUserRole<string> { UserId = "1", RoleId = "1" },
                new IdentityUserRole<string> { UserId = "2", RoleId = "1" },
                new IdentityUserRole<string> { UserId = "3", RoleId = "2" }
            );
            context.Properties.AddRange(
                new Property { Id = 1, LandlordId = "1", CityId = 1, Address = "Address1", Description = "Description1", Price = 1000, Status = PropertyStatus.Available },
                new Property { Id = 2, LandlordId = "1", CityId = 2, Address = "Address2", Description = "Description2", Price = 2000, Status = PropertyStatus.Occupied },
                new Property { Id = 3, LandlordId = "2", CityId = 2, Address = "Address3", Description = "Description3", Price = 3000, Status = PropertyStatus.Occupied }
            );
            context.Photos.AddRange(
                new Photo { Id = "1", PropertyId = 1 },
                new Photo { Id = "2", PropertyId = 1 },
                new Photo { Id = "3", PropertyId = 2 },
                new Photo { Id = "4", PropertyId = 2 },
                new Photo { Id = "5", PropertyId = 3 }
            );
            context.Responses.AddRange(
                new Response { Id = 1, TenantId = "3", PropertyId = 1, Message = "Message1", Status = ResponseStatus.ApprovedToRent },
                new Response { Id = 2, TenantId = "3", PropertyId = 2, Message = "Message2", Status = ResponseStatus.ApprovedToDialog },
                new Response { Id = 3, TenantId = "3", PropertyId = 3, Message = "Message3", Status = ResponseStatus.ApprovedToRent }
            );
            context.Comments.AddRange(
                new Comment { Id = 1, TenantId = "2", PropertyId = 1, Message = "Message1", Rate = Rate.Average },
                new Comment { Id = 2, TenantId = "2", PropertyId = 3, Message = "Message2", Rate = Rate.Average }
            );

            context.SaveChanges();
        }
    }
}
