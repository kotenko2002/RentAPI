using Rent.Entities.Cities;
using Rent.Entities.Comments;
using Rent.Storage.Configuration;
using Rent.Storage.Configuration.BaseRepository;

namespace Rent.Storage.Repositories.Comments
{
    public class CommentRepository : BaseRepository<Comment>, ICommentRepository
    {
        public CommentRepository(ApplicationDbContext context) : base(context)
        {
        }
    }
}
