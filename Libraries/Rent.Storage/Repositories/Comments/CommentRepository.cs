using Microsoft.EntityFrameworkCore;
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

        public async Task<IEnumerable<Comment>> GetFullCommentsByPropertyIdAsync(int propertyId)
        {
            return await Sourse
                .Include(c => c.Tenant)
                .Where(c => c.PropertyId == propertyId)
                .ToListAsync();
        }
    }
}
