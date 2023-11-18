namespace Rent.Storage.Uow
{
    public interface IUnitOfWork
    {
        Task CompleteAsync();
    }
}
