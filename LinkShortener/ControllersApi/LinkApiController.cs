using LinkShortener.DAL.ViewModels;
using LinkShortener.Service.Interfaces;
using Microsoft.AspNetCore.Mvc;

namespace LinkShortener.ControllersApi;

[ApiController]
[Route("api/v1/[controller]")]
public class LinkApiController : ControllerBase
{
    private readonly ILinkService _linkService;
    public LinkApiController(ILinkService linkService)
    {
        _linkService = linkService;
    }
    
    [HttpGet("get-links")]
    public async Task<IActionResult> GetLinks()
    {
        var links = await _linkService.GetLinksAsync();
        return Ok(links);
    }
    
    [HttpGet("get-link-by-id/{id}")]
    public async Task<IActionResult> GetLinkById(int id)
    {
        var link = await _linkService.GetLinkByIdAsync(id);
        return Ok(link);
    }
    
    [HttpPost]
    public async Task<IActionResult> CreateLink([FromBody]LongUrlViewModel longUrlViewModel)
    {
        var result = await _linkService.CreateLinkAsync(longUrlViewModel);
        if (!result)
        {
            return BadRequest();
        }

        return Ok();
    }
    
    [HttpPut]
    public async Task<IActionResult> UpdateLink([FromBody]LongUrlViewModel longUrlViewModel)
    {
        var result = await _linkService.UpdateLinkAsync(longUrlViewModel);
        if (!result)
        {
            return BadRequest();
        }
        
        return Ok("Update is success.");
    }
    
    [HttpDelete("delete-link-by-id/{id}")] 
    public async Task<IActionResult> Delete(int id)
    {
        var isDeleteLink = await _linkService.DeleteLinkAsync(id);
        if (!isDeleteLink)
        {
            return BadRequest();
        } 
        
        return Ok("Delete is successful.");
    }
}