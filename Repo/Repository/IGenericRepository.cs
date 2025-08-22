namespace Repo.Repository;

public interface IGenericRepository<T,Response, TKey> where T : class
{
    Task<IEnumerable<Response>> GetAllAsync();
    Task<T> GetById(TKey id);
    Task Create(T entity);
    Task Update(T entity);
    Task Delete(TKey id);
}