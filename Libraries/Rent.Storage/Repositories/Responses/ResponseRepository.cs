using Microsoft.EntityFrameworkCore;
using Rent.Entities.Responses;
using Rent.Storage.Configuration;
using Rent.Storage.Configuration.BaseRepository;

namespace Rent.Storage.Repositories.Responses
{
    public class ResponseRepository : BaseRepository<Response>, IResponseRepository
    {
        public ResponseRepository(ApplicationDbContext context) : base(context)
        {
        }

        public async Task<Response> GetFullResponseById(int propertyId)
        {
            return await Sourse
                .Include(r => r.Property)
                .FirstOrDefaultAsync(r => r.Id == propertyId);
        }
    }
}
