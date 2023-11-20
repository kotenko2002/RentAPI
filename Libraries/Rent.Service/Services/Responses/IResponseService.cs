using Rent.Entities.Responses;
using Rent.Service.Services.Responses.Views;

namespace Rent.Service.Services.Responses
{
    public interface IResponseService
    {
        Task Add(Response entity);
        Task<IEnumerable<ResponseView>> GetAllResponsesByPropertyId(int propertyId, string landlordId);
        Task Process();
    }
}
