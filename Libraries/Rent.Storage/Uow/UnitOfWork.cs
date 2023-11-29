using Rent.Storage.Configuration;
using Rent.Storage.Repositories.Cities;
using Rent.Storage.Repositories.Comments;
using Rent.Storage.Repositories.Photos;
using Rent.Storage.Repositories.Properties;
using Rent.Storage.Repositories.Responses;

namespace Rent.Storage.Uow
{
    public class UnitOfWork : IUnitOfWork, IDisposable
    {
        private readonly ApplicationDbContext _context;

        public ICityRepository CityRepository { get; }
        public IPropertyRepository PropertyRepository { get; }
        public IPhotoRepository PhotoRepository { get; }
        public IResponseRepository ResponseRepository { get; }
        public ICommentRepository CommentRepository { get; }

        public UnitOfWork(ApplicationDbContext context)
        {
            _context = context;

            CityRepository = new CityRepository(_context);
            PropertyRepository = new PropertyRepository(_context);
            PhotoRepository = new PhotoRepository(_context);
            ResponseRepository = new ResponseRepository(_context);
            CommentRepository = new CommentRepository(_context);
        }

        public async Task CompleteAsync()
        {
            await _context.SaveChangesAsync();
        }

        public void Dispose()
        {
            _context.Dispose();
        }
    }
}
