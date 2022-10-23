using LinkShortener.DAL;
using LinkShortener.DAL.Models;
using LinkShortener.DAL.ViewModels;

namespace LinkShortener.Service;

public interface ILinkService
{
    Task<bool> DeleteLink(int id);
    Task<bool> UpdateLink(LongUrlViewModel longUrlViewModel);
    Task<List<Link>> GetLinks();
    Task<Link?> GetLinkById(int id);
    Task<bool> CreateLink(LongUrlViewModel longUrlViewModel);
    string ReduceUrl(string host, string? longUrl);
    Task<string?> GetLongUrlByShortUrl(string shortUrl);
}