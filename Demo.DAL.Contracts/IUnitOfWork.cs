namespace Demo.DAL.Contracts
{
    public interface IUnitOfWork
    {
        void Commit();
        IRepository GetRepository<T>() where T : class;
    }
}

