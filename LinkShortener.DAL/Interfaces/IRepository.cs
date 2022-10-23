namespace LinkShortener.DAL.Interfaces;

public interface IRepository<T>
{
    Task Create(T model);
    IQueryable<T> GetAll();
    Task<T?> GetById(int id);
    Task<T> Update(T model);
    Task Delete(T model);
}