using Rent.Entities.Comments;
using Rent.Service.Services.Comments.Views;

namespace Rent.Service.Services.Comments
{
    public interface ICommentService
    {
        Task Add(Comment entity);
        Task Delete(int commentId, string tenantId);
        Task<IEnumerable<CommentView>> GetCommentsByPropertyId(int propertyId);
    }
}
