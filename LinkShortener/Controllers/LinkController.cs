using LinkShortener.DAL.ViewModels;
using LinkShortener.Service;
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
            var links = await _linkService.GetLinks();
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
            
            var link = await _linkService.GetLinkById(id);
            if (link == null)
            {
                return BadRequest("Такого элемента не существует");
            }
            
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
                bool isSuccess;
                if (longUrlViewModel.Id == 0)
                {
                    isSuccess = await _linkService.CreateLink(longUrlViewModel);
                }
                else
                {
                    isSuccess = await _linkService.UpdateLink(longUrlViewModel);
                }

                if (!isSuccess)
                {
                    return StatusCode(500);
                }
                
                return RedirectToAction("Index"); 
            }

            return View(longUrlViewModel);
        }
        
        public async Task<IActionResult> Delete(int id)
        {
            var isDeleteLink = await _linkService.DeleteLink(id);
            if (!isDeleteLink)
            {
                return BadRequest("Такого элемента не существует");
            }
            
            return RedirectToAction("Index");
        }
        
        public async Task<IActionResult> RedirectToLongUrl(string url)
        {
            var longUrl = await _linkService.GetLongUrlByShortUrl(url);
            if (string.IsNullOrEmpty(longUrl))
            {
                return NotFound("Такой сокращенной ссылки нет в базе данных");
            }
            
            return Redirect(longUrl);
        }
    }
}