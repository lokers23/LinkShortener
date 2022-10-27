using LinkShortener.DAL.Models;
using LinkShortener.DAL.ViewModels;

namespace LinkShortener.Service.Interfaces;

public interface ILinkService
{
    Task<bool> DeleteLinkAsync(int id);
    Task<bool> UpdateLinkAsync(LongUrlViewModel longUrlViewModel);
    Task<List<Link>> GetLinksAsync();
    Task<Link?> GetLinkByIdAsync(int id);
    Task<bool> CreateLinkAsync(LongUrlViewModel longUrlViewModel);
    string ReduceUrl(string host, string? longUrl);
    Task<string?> GetLongUrlByShortUrlAsync(string shortUrl);
}