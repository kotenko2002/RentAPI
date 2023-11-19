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
    }
}
