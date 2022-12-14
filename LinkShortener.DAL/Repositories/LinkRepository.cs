using LinkShortener.DAL.Interfaces;
using LinkShortener.Domain.Models;
using Microsoft.EntityFrameworkCore;

namespace LinkShortener.DAL.Repositories;

public class LinkRepository: IRepository<Link>
{
    private readonly ShortLinkDbContext _db;
    public LinkRepository(ShortLinkDbContext context)
    {
        _db = context;
    }

    public async Task CreateAsync(Link model)
    {
        await _db.Links.AddAsync(model);
        await _db.SaveChangesAsync();
    }

    public IQueryable<Link> GetAll()
    {
        return _db.Links;
    }

    public async Task<Link?> GetByIdAsync(int id)
    {
        return await _db.Links.FirstOrDefaultAsync(x => x.Id == id);
    }

    public async Task<Link> UpdateAsync(Link model)
    {
        _db.Links.Update(model);
        await _db.SaveChangesAsync();
        return model;
    }

    public async Task DeleteAsync(Link model)
    {
        _db.Links.Remove(model);
        await _db.SaveChangesAsync();
    }
}