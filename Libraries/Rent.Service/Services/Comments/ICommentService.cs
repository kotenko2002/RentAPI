using Rent.Entities.Comments;
using Rent.Service.Services.Comments.Views;

namespace Rent.Service.Services.Comments
{
    public interface ICommentService
    {
        Task AddAsync(Comment entity);
        Task DeleteAsync(int commentId, string tenantId);
        Task<IEnumerable<CommentView>> GetCommentsByPropertyIdAsync(int propertyId);
    }
}
