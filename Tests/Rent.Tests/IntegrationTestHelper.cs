
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Mvc.Testing;
using Microsoft.AspNetCore.TestHost;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Options;
using Microsoft.IdentityModel.Tokens;
using Rent.Entities.Cities;
using Rent.Entities.Comments;
using Rent.Entities.Photos;
using Rent.Entities.Properties;
using Rent.Entities.Responses;
using Rent.Entities.Users;
using Rent.Service.Configuration;
using Rent.Storage.Configuration;
using System.IdentityModel.Tokens.Jwt;
using System.Security.Claims;
using System.Text;

namespace Rent.Tests
{
    public class IntegrationTestHelper
    {
        public WebApplicationFactory<RentAPI.Program> GetWebApplicationFactory(string databaseName)
        {
            var factory = new WebApplicationFactory<RentAPI.Program>().WithWebHostBuilder(builder =>
                {
                    builder.ConfigureTestServices(services =>
                    {
                        var dbContextDescriptor = services.SingleOrDefault(d =>
                            d.ServiceType == typeof(DbContextOptions<ApplicationDbContext>));

                        services.Remove(dbContextDescriptor);

                        services.AddDbContext<ApplicationDbContext>(options =>
                        {
                            options.UseInMemoryDatabase(databaseName);
                        });
                    });
                });

            using var scope = factory.Services.CreateScope(); ;
            var db = scope.ServiceProvider.GetRequiredService<ApplicationDbContext>();
            var userManager = scope.ServiceProvider.GetRequiredService<UserManager<User>>();
            var roleManager = scope.ServiceProvider.GetRequiredService<RoleManager<IdentityRole>>();

            SeedData(userManager, roleManager, db).Wait();

            return factory;
        }

        public async Task<string> GenerateAccessToken(WebApplicationFactory<RentAPI.Program> factory, string username)
        {
            using var scope = factory.Services.CreateScope();
            var config = scope.ServiceProvider.GetRequiredService<IOptions<JwtConfig>>().Value;
            var userManager = scope.ServiceProvider.GetRequiredService<UserManager<User>>();

            User user = await userManager.FindByNameAsync(username);

            var authClaims = new List<Claim>
            {
                new Claim("userId", user.Id),
                new Claim(ClaimTypes.Name, user.UserName),
                new Claim(JwtRegisteredClaimNames.Jti, Guid.NewGuid().ToString()),
            };
            IList<string> userRoles = await userManager.GetRolesAsync(user);
         
            authClaims.AddRange(userRoles.Select(role => new Claim(ClaimTypes.Role, role)));

            var authSigningKey = new SymmetricSecurityKey(Encoding.UTF8.GetBytes(config.Secret));

            var accessToken = new JwtSecurityToken(
                issuer: config.ValidIssuer,
                audience: config.ValidAudience,
                expires: DateTime.Now.AddMinutes(config.TokenValidityInMinutes),
                claims: authClaims,
                signingCredentials: new SigningCredentials(authSigningKey, SecurityAlgorithms.HmacSha256)
            );

            return new JwtSecurityTokenHandler().WriteToken(accessToken);
        }
        private async Task SeedData(UserManager<User> userManager, RoleManager<IdentityRole> roleManager, ApplicationDbContext context)
        {
            await roleManager.CreateAsync(new IdentityRole(Roles.Landlord));
            await roleManager.CreateAsync(new IdentityRole(Roles.Tenant));

            var landlord1 = await AddUser(userManager, "landlord1", "landlord1@example.com", Roles.Landlord);
            var landlord2 = await AddUser(userManager, "landlord2", "landlord2@example.com", Roles.Landlord);
            var tenant1 = await AddUser(userManager, "tenant1", "tenant1@example.com", Roles.Tenant);
            var tenant2 = await AddUser(userManager, "tenant2", "tenant2@example.com", Roles.Tenant);

            context.Cities.AddRange(
                new City { Id = 1, Name = "City1" },
                new City { Id = 2, Name = "City2" }
            );
            context.Properties.AddRange(
                new Property { Id = 1, Landlord = landlord1, CityId = 1, Address = "Address1", Description = "Description1", Price = 1000, Status = PropertyStatus.Available },
                new Property { Id = 2, Landlord = landlord1, CityId = 2, Address = "Address2", Description = "Description2", Price = 2000, Status = PropertyStatus.Occupied },
                new Property { Id = 3, Landlord = landlord2, CityId = 2, Address = "Address3", Description = "Description3", Price = 3000, Status = PropertyStatus.Occupied }
            );
            context.Photos.AddRange(
                new Photo { Id = "1", PropertyId = 1 },
                new Photo { Id = "2", PropertyId = 1 },
                new Photo { Id = "3", PropertyId = 2 },
                new Photo { Id = "4", PropertyId = 2 },
                new Photo { Id = "5", PropertyId = 3 }
            );
            context.Responses.AddRange(
                new Response { Id = 1, Tenant = tenant1, PropertyId = 1, Message = "Message1", Status = ResponseStatus.ApprovedToRent },
                new Response { Id = 2, Tenant = tenant1, PropertyId = 2, Message = "Message2", Status = ResponseStatus.ApprovedToDialog },
                new Response { Id = 3, Tenant = tenant1, PropertyId = 3, Message = "Message3", Status = ResponseStatus.ApprovedToRent },
                new Response { Id = 4, Tenant = tenant2, PropertyId = 1, Message = "Message4", Status = ResponseStatus.NotReviewed }
            );
            context.Comments.AddRange(
                new Comment { Id = 1, Tenant = tenant1, PropertyId = 1, Message = "Message1", Rate = Rate.Average },
                new Comment { Id = 2, Tenant = tenant1, PropertyId = 3, Message = "Message2", Rate = Rate.Average }
            );

            await context.SaveChangesAsync();
        }
        private async Task<User> AddUser(UserManager<User> userManager, string username, string email, string role)
        {
            var user = new User { UserName = username, Email = email, RefreshToken = "refresh_token", RefreshTokenExpiryTime = DateTime.Now.AddDays(1) };
            await userManager.CreateAsync(user, "Qwerty123!");
            await userManager.AddToRoleAsync(user, role);
            return user;
        }
    }
}
