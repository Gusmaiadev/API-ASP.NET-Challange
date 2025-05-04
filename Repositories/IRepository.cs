namespace DentalClinicAPI.Repositories
{
    // Interface base para operações de leitura
    public interface IReadRepository<T> where T : class
    {
        Task<IEnumerable<T>> GetAll();
        Task<T?> GetById(int id);
    }   

    // Interface para operações de escrita
    public interface IWriteRepository<T> where T : class
    {
        Task Create(T entity);
        Task Update(T entity);
        Task Delete(int id);
    }

    // Interface completa que herda as duas anteriores
    public interface IRepository<T> : IReadRepository<T>, IWriteRepository<T> where T : class
    {
    }
}