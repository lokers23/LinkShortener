using LinkShortener.Domain.Response;
using LinkShortener.Domain.ViewModels;
using LinkShortener.Service;
using LinkShortener.Service.Interfaces;
using Microsoft.AspNetCore.Mvc;

namespace LinkShortener.Controllers
{
    public class LinkController : Controller
    {
        private readonly ILinkService _linkService;
        public LinkController(ILinkService linkService)
        {
            _linkService = linkService;
        }
        public async Task<IActionResult> Index()
        {
            var links = await _linkService.GetLinksAsync();
            return View(links);
        }

        [HttpGet]
        public async Task<IActionResult> Save(int id)
        {
            var longUrlViewModel = new LongUrlViewModel();
            if (id == 0)
            {
                return View(longUrlViewModel);
            }
            
            var response = await _linkService.GetLinkByIdAsync(id);
            if (response.StatusCode != Domain.Enums.StatusCode.Success)
            {
                return BadRequest(response.Message);
            }

            var link = response.Data!;
           
            longUrlViewModel.Id = link.Id;
            longUrlViewModel.LongUrl = link.LongUrl;
            longUrlViewModel.ShortUrl = link.ShortUrl;
            longUrlViewModel.DateCreate = link.DateCreate;
            longUrlViewModel.CountClick = link.CountClick;
            
            return View(longUrlViewModel);
        }
        
        [HttpPost]
        public async Task<IActionResult> Save(LongUrlViewModel longUrlViewModel)
        {
            if (ModelState.IsValid)
            {
                Response<bool> response;
                if (longUrlViewModel.Id == 0)
                {
                    response = await _linkService.CreateLinkAsync(longUrlViewModel);
                }
                else
                {
                    response = await _linkService.UpdateLinkAsync(longUrlViewModel);
                }

                if (response.StatusCode != Domain.Enums.StatusCode.Success)
                {
                    return BadRequest(response.Message);
                }
                
                return RedirectToAction("Index"); 
            }

            return View(longUrlViewModel);
        }
        
        public async Task<IActionResult> Delete(int id)
        {
            var response = await _linkService.DeleteLinkAsync(id);
            if (response.StatusCode != Domain.Enums.StatusCode.Success)
            {
                return BadRequest(response.Message);
            }
            
            return RedirectToAction("Index");
        }
        
        public async Task<IActionResult> RedirectToLongUrl(string url)
        {
            var response = await _linkService.GetLongUrlByShortUrlAsync(url);
            if (response.StatusCode != Domain.Enums.StatusCode.Success)
            {
                return BadRequest(response.Message);
            }
            
            return Redirect(response.Data!);
        }
    }
}