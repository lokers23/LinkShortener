namespace LinkShortener.DAL.Interfaces;

public interface IRepository<T>
{
    Task CreateAsync(T model);
    IQueryable<T> GetAll();
    Task<T?> GetByIdAsync(int id);
    Task<T> UpdateAsync(T model);
    Task DeleteAsync(T model);
}