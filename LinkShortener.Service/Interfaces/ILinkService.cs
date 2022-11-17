using LinkShortener.Domain.Models;
using LinkShortener.Domain.Response;
using LinkShortener.Domain.ViewModels;

namespace LinkShortener.Service.Interfaces;

public interface ILinkService
{
    Task<Response<bool>> DeleteLinkAsync(int id);
    Task<Response<bool>> UpdateLinkAsync(LongUrlViewModel longUrlViewModel);
    Task<Response<List<Link>>> GetLinksAsync();
    Task<Response<Link>> GetLinkByIdAsync(int id);
    Task<Response<bool>> CreateLinkAsync(LongUrlViewModel longUrlViewModel);
    string ReduceUrl(string host, string longUrl);
    Task<Response<string>> GetLongUrlByShortUrlAsync(string shortUrl);
}