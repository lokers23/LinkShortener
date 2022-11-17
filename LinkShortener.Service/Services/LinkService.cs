using System.ComponentModel.DataAnnotations;
using HashidsNet;
using LinkShortener.DAL.Interfaces;
using LinkShortener.DAL.Models;
using LinkShortener.DAL.ViewModels;
using LinkShortener.Service.Interfaces;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Logging;

namespace LinkShortener.Service.Services;

public class LinkService : ILinkService
{
    private const string HostUrl = "https://localhost:7288/";
    private readonly IRepository<Link> _linkRepository;
    private readonly ILogger<LinkService> _logger;

    public LinkService(ILogger<LinkService> logger, IRepository<Link> linkRepository)
    {
        _logger = logger;
        _linkRepository = linkRepository;
    }

    public async Task<bool> DeleteLinkAsync(int id)
    {
        try
        {
            var link = await _linkRepository.GetByIdAsync(id);

            if (link == null)
            {
                return false;
            }

            await _linkRepository.DeleteAsync(link);
            return true;
        }
        catch (Exception exception)
        {
            _logger.LogError(exception, "{message}", exception.Message);
            return false;
        }
    }

    public async Task<bool> UpdateLinkAsync(LongUrlViewModel longUrlViewModel)
    {
        try
        {
            if (longUrlViewModel.Id <= 0)
            {
                throw new ValidationException($"Can not update element with id equal to {longUrlViewModel.Id}.");
            }
            
            var linkFromDb = await _linkRepository.GetByIdAsync(longUrlViewModel.Id);
            if (linkFromDb == null)
            {
                return false;
            }

            linkFromDb.LongUrl = longUrlViewModel.LongUrl;
            linkFromDb.ShortUrl = ReduceUrl(HostUrl, longUrlViewModel.LongUrl);
            linkFromDb.DateCreate = longUrlViewModel.DateCreate;
            linkFromDb.CountClick = longUrlViewModel.CountClick;

            await _linkRepository.UpdateAsync(linkFromDb);
            return true;
        }
        catch (Exception exception)
        {
            _logger.LogError(exception, "{message}", exception.Message);
            return false;
        }
    }

    public async Task<List<Link>> GetLinksAsync()
    {
        try
        {
            var links = await _linkRepository.GetAll().ToListAsync();
            return links;
        }
        catch (Exception exception)
        {
            _logger.LogError(exception, "{message}", exception.Message);
            return new List<Link>();
        }
    }

    public async Task<Link?> GetLinkByIdAsync(int id)
    {
        try
        {
            if (id <= 0)
            {
                throw new ValidationException($"Id equal to {id} does not exist.");
            }
            
            var link = await _linkRepository.GetByIdAsync(id);
            return link;
        }
        catch (Exception exception)
        {
            _logger.LogError(exception, "{message}", exception.Message);
            return null;
        }
    }

    public async Task<bool> CreateLinkAsync(LongUrlViewModel longUrlViewModel)
    {
        try
        {
            if (longUrlViewModel.Id != 0)
            {
                throw new ValidationException($"Can not create element with id equal to {longUrlViewModel.Id}.");
            }
            
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

                await _linkRepository.CreateAsync(link);
            }

            return true;
        }
        catch (Exception exception)
        {
            _logger.LogError(exception, "{message}", exception.Message);
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

    public async Task<string?> GetLongUrlByShortUrlAsync(string shortUrl)
    {
        try
        {
            var link = await _linkRepository.GetAll().FirstOrDefaultAsync(url => url.ShortUrl == shortUrl);
            if (link == null)
            {
                return null;
            }

            await UpCountOfLinkClicksAsync(link, 1);
            return link.LongUrl;
        }
        catch (Exception exception)
        {
            _logger.LogError(exception, "{message}", exception.Message);
            return null;
        }
    }

    private async Task UpCountOfLinkClicksAsync(Link link, int upNumber)
    {
        link.CountClick += upNumber;
        await _linkRepository.UpdateAsync(link);
    }

}