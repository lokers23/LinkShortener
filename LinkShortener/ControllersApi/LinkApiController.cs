using LinkShortener.Domain.Response;
using LinkShortener.Domain.ViewModels;
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
        var response = await _linkService.GetLinksAsync();
        return Ok(response);
    }
    
    [HttpGet("get-link-by-id/{id}")]
    public async Task<IActionResult> GetLinkById(int id)
    {
        var response = await _linkService.GetLinkByIdAsync(id);
        if (response.StatusCode != Domain.Enums.StatusCode.Success)
        {
            return BadRequest(response.Message);
        }

        return Ok(response);
    }
    
    [HttpPost]
    public async Task<IActionResult> CreateLink([FromBody]LongUrlViewModel longUrlViewModel)
    {
        var response = await _linkService.CreateLinkAsync(longUrlViewModel);
        if (response.StatusCode != Domain.Enums.StatusCode.Success)
        {
            return BadRequest(response.Message);
        }

        return Ok(response);
    }
    
    [HttpPut]
    public async Task<IActionResult> UpdateLink([FromBody]LongUrlViewModel longUrlViewModel)
    {
        var response = await _linkService.UpdateLinkAsync(longUrlViewModel);
        if (response.StatusCode != Domain.Enums.StatusCode.Success)
        {
            return BadRequest(response.Message);
        }

        return Ok(response);
    }
    
    [HttpDelete("delete-link-by-id/{id}")] 
    public async Task<IActionResult> Delete(int id)
    {
        var response = await _linkService.DeleteLinkAsync(id);
        if (response.StatusCode != Domain.Enums.StatusCode.Success)
        {
            return BadRequest(response.Message);
        }

        return Ok(response);
    }
}