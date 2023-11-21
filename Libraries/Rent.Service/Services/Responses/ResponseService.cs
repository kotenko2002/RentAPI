using Rent.Common;
using Rent.Entities.Properties;
using Rent.Entities.Responses;
using Rent.Service.Services.Responses.Descriptors;
using Rent.Service.Services.Responses.Views;
using Rent.Storage.Uow;
using System.Net;

namespace Rent.Service.Services.Responses
{
    public class ResponseService : IResponseService
    {
        private readonly IUnitOfWork _uow;

        public ResponseService(IUnitOfWork uow)
        {
            _uow = uow;
        }

        public async Task Add(Response entity)
        {
            entity.Status = ResponseStatus.NotReviewed;
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

            IEnumerable<ResponseView> resposes = entity.Responses.Select(res => new ResponseView()
            {
                Id = res.Id,
                Email = res.Tenant.Email,
                PhoneNumber = res.Tenant.PhoneNumber,
                Message = res.Message,
                Status = res.Status
            }); 

            return resposes;
        }

        public async Task Process(ProcessResponseDescriptor descriptor)
        {
            Response entity = await _uow.ResponseRepository.GetFullResponseById(descriptor.ResponseId);

            if (entity.Property.LandlordId != descriptor.LandlordId)
            {
                throw new BusinessException(HttpStatusCode.Forbidden,
                    "Access denied. You do not have permission to process this response.");
            }

            entity.Status = descriptor.Status;
            await _uow.CompleteAsync();
        }
    }
}
