using Rent.Entities.Comments;
using Rent.Storage.Configuration.BaseRepository;

namespace Rent.Storage.Repositories.Comments
{
    public interface ICommentRepository : IBaseRepository<Comment>
    {
        Task<IEnumerable<Comment>> GetFullCommentsByPropertyIdAsync(int propertyId);
    }
}
