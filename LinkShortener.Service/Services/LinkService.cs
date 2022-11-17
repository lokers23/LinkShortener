using System;
using System.ComponentModel.DataAnnotations;
using System.Xml.Linq;
using HashidsNet;
using LinkShortener.DAL.Interfaces;
using LinkShortener.Domain.Models;
using LinkShortener.Domain.Response;
using LinkShortener.Domain.ViewModels;
using LinkShortener.Service.Interfaces;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Logging;
using static Microsoft.EntityFrameworkCore.DbLoggerCategory;

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

    public async Task<Response<bool>> DeleteLinkAsync(int id)
    {
        try
        {
            var link = await _linkRepository.GetByIdAsync(id);

            if (link == null)
            {
                return new Response<bool>
                {
                    Data = false,
                    Message = $"Not found link with id equal to {id}.",
                    StatusCode = Domain.Enums.StatusCode.NotFound
                };
            }

            await _linkRepository.DeleteAsync(link);
            return new Response<bool>
            {
                Data = true,
                StatusCode = Domain.Enums.StatusCode.Success
            };
        }
        catch (Exception exception)
        {
            _logger.LogError(exception, "{message}.", exception.Message);
            return new Response<bool>
            {
                Data = false,
                Message = $"{exception.Message}.",
                StatusCode = Domain.Enums.StatusCode.ServerError
            };
        }
    }

    public async Task<Response<bool>> UpdateLinkAsync(LongUrlViewModel longUrlViewModel)
    {
        try
        {
            if (longUrlViewModel.Id <= 0)
            {
                return new Response<bool>
                {
                    Data = false,
                    Message = $"Can not update element with id equal to { longUrlViewModel.Id }.",
                    StatusCode = Domain.Enums.StatusCode.ValidationError
                };
            }
            
            var linkFromDb = await _linkRepository.GetByIdAsync(longUrlViewModel.Id);
            if (linkFromDb == null)
            {
                return new Response<bool> 
                {
                    Data = false , 
                    Message= $"Not found link with id equal to {longUrlViewModel.Id}.", 
                    StatusCode = Domain.Enums.StatusCode.NotFound
                };
            }

            linkFromDb.LongUrl = longUrlViewModel.LongUrl;
            linkFromDb.ShortUrl = ReduceUrl(HostUrl, longUrlViewModel.LongUrl);
            linkFromDb.DateCreate = longUrlViewModel.DateCreate;
            linkFromDb.CountClick = longUrlViewModel.CountClick;

            await _linkRepository.UpdateAsync(linkFromDb);
            return new Response<bool>
            {
                Data = true,
                StatusCode = Domain.Enums.StatusCode.Success
            };
        }
        catch (Exception exception)
        {
            _logger.LogError(exception, "{message}.", exception.Message);
            return new Response<bool>
            {
                Data = false,
                Message = $"{exception.Message}.",
                StatusCode = Domain.Enums.StatusCode.ServerError
            };
        }
    }

    public async Task<Response<List<Link>>> GetLinksAsync()
    {
        try
        {
            var links = await _linkRepository.GetAll().ToListAsync();
            return new Response<List<Link>>
            {
                Data = links,
                StatusCode = Domain.Enums.StatusCode.Success
            };
        }
        catch (Exception exception)
        {
            _logger.LogError(exception, "{message}.", exception.Message);
            return new Response<List<Link>>
            {
                Data = new List<Link>(),
                Message = $"{exception.Message}.",
                StatusCode = Domain.Enums.StatusCode.ServerError
            };
        }
    }

    public async Task<Response<Link>> GetLinkByIdAsync(int id)
    {
        try
        {
            if (id <= 0)
            {
                return new Response<Link>
                {
                    Data = null,
                    Message = $"Id equal to {id} is invalid.",
                    StatusCode = Domain.Enums.StatusCode.ValidationError
                };
            }
            
            var link = await _linkRepository.GetByIdAsync(id);
            if (link == null)
            {
                return new Response<Link>
                {
                    Data = null,
                    Message = $"Link with id equal to {id} does not exist.",
                    StatusCode = Domain.Enums.StatusCode.ValidationError
                };
            }

            return new Response<Link>
            {
                Data = link,
                StatusCode = Domain.Enums.StatusCode.Success
            };
        }
        catch (Exception exception)
        {
            _logger.LogError(exception, "{message}.", exception.Message);
            return new Response<Link>
            {
                Data = null,
                Message = $"{exception.Message}.",
                StatusCode = Domain.Enums.StatusCode.ServerError
            };
        }
    }

    public async Task<Response<bool>> CreateLinkAsync(LongUrlViewModel longUrlViewModel)
    {
        try
        {
            if (longUrlViewModel.Id != 0)
            {
                return new Response<bool>
                {
                    Data = false,
                    Message = $"Can not create element with id equal to {longUrlViewModel.Id}.",
                    StatusCode = Domain.Enums.StatusCode.ValidationError
                };
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

            return new Response<bool>
            {
                Data = true,
                StatusCode = Domain.Enums.StatusCode.Success
            };
        }
        catch (Exception exception)
        {
            _logger.LogError(exception, "{message}.", exception.Message);
            return new Response<bool>
            {
                Data = false,
                Message = $"{exception.Message}.",
                StatusCode = Domain.Enums.StatusCode.ServerError
            };
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

    public async Task<Response<string>> GetLongUrlByShortUrlAsync(string shortUrl)
    {
        try
        {
            var link = await _linkRepository.GetAll().FirstOrDefaultAsync(url => url.ShortUrl == shortUrl);
            if (link == null)
            {
                return new Response<string>
                {
                    Data = null,
                    Message = $"Not found long url with short url equal to {shortUrl}.",
                    StatusCode = Domain.Enums.StatusCode.NotFound
                };
            }

            await UpCountOfLinkClicksAsync(link, 1);
            return new Response<string>
            {
                Data = link.LongUrl,
                StatusCode = Domain.Enums.StatusCode.Success
            };
        }
        catch (Exception exception)
        {
            _logger.LogError(exception, "{message}.", exception.Message);
            return new Response<string>
            {
                Data = null,
                Message = $"{exception.Message}.",
                StatusCode = Domain.Enums.StatusCode.ServerError
            };
        }
    }

    private async Task UpCountOfLinkClicksAsync(Link link, int upNumber)
    {
        link.CountClick += upNumber;
        await _linkRepository.UpdateAsync(link);
    }

}