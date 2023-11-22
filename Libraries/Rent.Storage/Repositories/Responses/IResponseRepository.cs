using Rent.Entities.Responses;
using Rent.Storage.Configuration.BaseRepository;

namespace Rent.Storage.Repositories.Responses
{
    public interface IResponseRepository : IBaseRepository<Response>
    {
        Task<Response> GetFullResponseById(int propertyId);
        Task<Response> GetResponseByPropertyAndTenantIds(int propertyId, string tenantId);
    }
}
