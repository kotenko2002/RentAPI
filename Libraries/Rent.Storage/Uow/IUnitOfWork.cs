using Rent.Storage.Repositories.Cities;
using Rent.Storage.Repositories.Comments;
using Rent.Storage.Repositories.Properties;
using Rent.Storage.Repositories.Responses;

namespace Rent.Storage.Uow
{
    public interface IUnitOfWork
    {
        ICityRepository CityRepository { get; }
        IPropertyRepository PropertyRepository { get; }
        IResponseRepository ResponseRepository { get; }
        ICommentRepository CommentRepository { get; }

        Task CompleteAsync();
    }
}
