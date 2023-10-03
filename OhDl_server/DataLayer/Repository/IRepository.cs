namespace OhDl_server.DataLayer.Repository;

public interface IRepository<TEntity> where TEntity : class
{
    public Task<TEntity?> Get(int Id);
    public Task<IEnumerable<TEntity>> GetAll();
    public Task Add(TEntity entity);
    public Task Update(TEntity entity);
    public Task Delete(TEntity entity);
}