using Rent.Common;
using System.Net;
using System.Security.Claims;

namespace RentAPI.Infrastructure.Extensions
{
    public static class ClaimsPrincipalExtensions
    {
        public static string GetUsername(this ClaimsPrincipal principal)
        {
            return GetInfoByDataName(principal, ClaimTypes.Name);
        }

        private static string GetInfoByDataName(ClaimsPrincipal principal, string name)
        {
            var data = principal.FindFirstValue(name);

            if (data == null)
            {
                throw new BusinessException(HttpStatusCode.InternalServerError, $"No such data as {name} in Token");
            }

            return data;
        }
    }
}
