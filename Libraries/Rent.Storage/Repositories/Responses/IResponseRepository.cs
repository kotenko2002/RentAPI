using Rent.Entities.Responses;
using Rent.Storage.Configuration.BaseRepository;

namespace Rent.Storage.Repositories.Responses
{
    public interface IResponseRepository : IBaseRepository<Response>
    {
        Task<Response> GetFullResponseByIdAsync(int propertyId);
        Task<Response> GetResponseByPropertyAndTenantIdsAsync(int propertyId, string tenantId);
    }
}
