using HashidsNet;
using LinkShortener.DAL.Interfaces;
using LinkShortener.DAL.Models;
using LinkShortener.DAL.ViewModels;
using Microsoft.EntityFrameworkCore;

namespace LinkShortener.Service.Services;

public class LinkService : ILinkService
{
    private const string HostUrl = "https://localhost:7288/";
    private readonly IRepository<Link> _linkRepository;

    public LinkService(IRepository<Link> linkRepository)
    {
        _linkRepository = linkRepository;
    }

    public async Task<bool> DeleteLink(int id)
    {
        try
        {
            var link = await _linkRepository.GetById(id);

            if (link == null)
            {
                return false;
            }

            await _linkRepository.Delete(link);
            return true;
        }
        catch (Exception)
        {
            return false;
        }
    }

    public async Task<bool> UpdateLink(LongUrlViewModel longUrlViewModel)
    {
        try
        {
            var linkFromDb = await _linkRepository.GetById(longUrlViewModel.Id);
            if (linkFromDb == null)
            {
                return false;
            }

            linkFromDb.LongUrl = longUrlViewModel.LongUrl;
            linkFromDb.ShortUrl = ReduceUrl(HostUrl, longUrlViewModel.LongUrl);
            linkFromDb.DateCreate = longUrlViewModel.DateCreate;
            linkFromDb.CountClick = longUrlViewModel.CountClick;

            await _linkRepository.Update(linkFromDb);
            return true;
        }
        catch (Exception)
        {
            return false;
        }
    }

    public async Task<List<Link>> GetLinks()
    {
        try
        {
            var links = await _linkRepository.GetAll().ToListAsync();
            return links;
        }
        catch (Exception)
        {
            return new List<Link>();
        }
    }

    public async Task<Link?> GetLinkById(int id)
    {
        try
        {
            var link = await _linkRepository.GetById(id);
            return link;
        }
        catch (Exception)
        {
            return null;
        }
    }

    public async Task<bool> CreateLink(LongUrlViewModel longUrlViewModel)
    {
        try
        {
            var isExists = await _linkRepository.GetAll().AnyAsync(link => link.LongUrl.Equals(longUrlViewModel.LongUrl));
            if (!isExists)
            {
                var link = new Link
                {
                    LongUrl = longUrlViewModel.LongUrl,
                    ShortUrl = ReduceUrl(HostUrl, longUrlViewModel.LongUrl),
                    DateCreate = DateTime.Now,
                    CountClick = 0
                };

                await _linkRepository.Create(link);
            }

            return true;
        }
        catch (Exception)
        {
            return false;
        }
    }

    public string ReduceUrl(string host, string? longUrl)
    {
        if (string.IsNullOrEmpty(longUrl))
        {
            throw new ArgumentException("LongUrl can not be null or empty.", longUrl);
        }

        var hashId = new Hashids(longUrl);
        var shortUrl = hashId.Encode(1, 2, 3, 4);

        return shortUrl;
    }

    public async Task<string?> GetLongUrlByShortUrl(string shortUrl)
    {
        try
        {
            var link = await _linkRepository.GetAll().FirstOrDefaultAsync(url => url.ShortUrl == shortUrl);
            if (link == null)
            {
                return null;
            }

            await UpCountOfLinkClicks(link, 1);
            return link.LongUrl;
        }
        catch (Exception)
        {
            return null;
        }
    }

    private async Task UpCountOfLinkClicks(Link link, int upNumber)
    {
        link.CountClick += upNumber;
        await _linkRepository.Update(link);
    }

}