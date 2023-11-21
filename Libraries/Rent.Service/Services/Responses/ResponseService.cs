using AutoMapper;
using Rent.Common;
using Rent.Entities.Properties;
using Rent.Entities.Responses;
using Rent.Service.Services.Responses.Views;
using Rent.Storage.Uow;
using System.Net;

namespace Rent.Service.Services.Responses
{
    public class ResponseService : IResponseService
    {
        private readonly IMapper _mapper;
        private readonly IUnitOfWork _uow;

        public ResponseService(
            IUnitOfWork uow,
            IMapper mapper)
        {
            _uow = uow;
            _mapper = mapper;
        }

        public async Task Add(Response entity)
        {
            await _uow.ResponseRepository.AddAsync(entity);
            await _uow.CompleteAsync();
        }

        public async Task<IEnumerable<ResponseView>> GetAllResponsesByPropertyId(int propertyId, string landlordId)
        {
            Property entity = await _uow.PropertyRepository.GetFullPropertyById(propertyId);

            if (entity == null)
            {
                throw new BusinessException(HttpStatusCode.NotFound, "Property not found.");
            }

            if (entity.LandlordId != landlordId)
            {
                throw new BusinessException(HttpStatusCode.Forbidden,
                    "Access denied. You do not have permission to get responses for this property.");
            }

            return _mapper.Map<IEnumerable<ResponseView>>(entity.Responses);
        }

        public Task Process()
        {
            throw new NotImplementedException();
        }
    }
}
