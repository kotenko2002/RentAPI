using Rent.Entities.Responses;
using Rent.Service.Services.Responses.Descriptors;
using Rent.Service.Services.Responses.Views;

namespace Rent.Service.Services.Responses
{
    public interface IResponseService
    {
        Task AddAsync(Response entity);
        Task<IEnumerable<ResponseView>> GetAllResponsesByPropertyIdAsync(int propertyId, string landlordId);
        Task ProcessAsync(ProcessResponseDescriptor descriptor);
    }
}
