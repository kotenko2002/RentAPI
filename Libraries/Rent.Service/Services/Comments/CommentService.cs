using AutoMapper;
using Rent.Common;
using Rent.Entities.Comments;
using Rent.Entities.Properties;
using Rent.Entities.Responses;
using Rent.Service.Services.Comments.Views;
using Rent.Storage.Uow;
using System.Net;

namespace Rent.Service.Services.Comments
{
    public class CommentService : ICommentService
    {
        private readonly IMapper _mapper;
        private readonly IUnitOfWork _uow;

        public CommentService(
            IMapper mapper,
            IUnitOfWork uow)
        {
            _mapper = mapper;
            _uow = uow;
        }

        public async Task AddAsync(Comment entity)
        {
            Response response = await _uow.ResponseRepository.GetResponseByPropertyAndTenantIdsAsync(
                entity.PropertyId, entity.TenantId);

            if (response == null || response.Status != ResponseStatus.ApprovedToRent)
            {
                throw new BusinessException(HttpStatusCode.Forbidden,
                    "Access denied. You do not have permission to comment this property.");
            }

            await _uow.CommentRepository.AddAsync(entity);
            await _uow.CompleteAsync();
        }

        public async Task<IEnumerable<CommentView>> GetCommentsByPropertyIdAsync(int propertyId)
        {
            Property property = await _uow.PropertyRepository.FindAsync(propertyId);

            if (property == null)
            {
                throw new BusinessException(HttpStatusCode.NotFound, "Property not found.");
            }

            IEnumerable<Comment> comments = await _uow.CommentRepository.GetFullCommentsByPropertyIdAsync(propertyId);

            return _mapper.Map<IEnumerable<CommentView>>(comments);
        }

        public async Task DeleteAsync(int commentId, string tenantId)
        {
            Comment entity = await _uow.CommentRepository.GetFullCommentByIdAsync(commentId);

            if (entity == null)
            {
                throw new BusinessException(HttpStatusCode.NotFound, "Comment not found.");
            }

            if (entity.TenantId != tenantId)
            {
                throw new BusinessException(HttpStatusCode.Forbidden,
                    "Access denied. You do not have permission to delete this comment.");
            }

            await _uow.CommentRepository.RemoveAsync(entity);
            await _uow.CompleteAsync();
        }
    }
}
