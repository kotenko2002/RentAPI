using Rent.Entities.Comments;
using Rent.Service.Services.Comments.Views;

namespace Rent.Service.Services.Comments
{
    public interface ICommentService
    {
        Task AddAsync(Comment entity);
        Task<IEnumerable<CommentView>> GetCommentsByPropertyIdAsync(int propertyId);
        Task DeleteAsync(int commentId, string tenantId);
    }
}
