using AutoFixture;
using LinkShortener.DAL.Interfaces;
using LinkShortener.DAL.Models;
using LinkShortener.Service.Services;
using Microsoft.Extensions.Logging;
using Moq;
using MockQueryable.Moq;

namespace LinkShortener.Service.Tests;

public class LinkServiceTests
{
    private readonly Mock<IRepository<Link>> _linkRepositoryMock;
    private readonly Mock<ILogger<LinkService>> _loggerMock;
    private LinkService _service;
    private readonly Fixture _fixture;
    public LinkServiceTests()
    {
        _fixture = new Fixture();
        
        _loggerMock = new Mock<ILogger<LinkService>>();
        _linkRepositoryMock = new Mock<IRepository<Link>>();
    }
    
    [Fact]
    public async Task Create_ShouldGetLinkById()
    {
        //Arrange
        const int linkId = 1;
        _service = new LinkService(_loggerMock.Object, _linkRepositoryMock.Object);
        
        //Act
        await _service.GetLinkByIdAsync(linkId);
        
        //Assert
        
        _linkRepositoryMock.Verify(x => x.GetByIdAsync(linkId), Times.Once);
    }

    [Fact]
    public async Task GetById_SomeId_ReturnLink()
    {
        //Arrange
        var link = _fixture.Create<Link>();

        _linkRepositoryMock.Setup(rep => rep.GetByIdAsync(link.Id)).ReturnsAsync(link);
        _service = new LinkService(_loggerMock.Object, _linkRepositoryMock.Object);
        
        //Act
        var result = await _service.GetLinkByIdAsync(link.Id);
        
        //Assert
        Assert.IsType<Link?>(result);
    }
    
    [Fact]
    public async Task GetById_ThrowException_ReturnNull()
    {
        //Arrange
        var link = _fixture.Create<Link>();

        _linkRepositoryMock.Setup(rep => rep.GetByIdAsync(link.Id)).ThrowsAsync(new Exception());
        _service = new LinkService(_loggerMock.Object, _linkRepositoryMock.Object);
        
        //Act
        var result = await _service.GetLinkByIdAsync(link.Id);
        
        //Assert
        Assert.Null(result);
    }

    [Fact]
    public async Task GetAll_ReturnLinks()
    {
        //Arrange
        var links = _fixture.CreateMany<Link>().AsQueryable().BuildMock();
        
        _linkRepositoryMock.Setup(rep => rep.GetAll()).Returns(links);
        _service = new LinkService(_loggerMock.Object, _linkRepositoryMock.Object);
        
        //Act
        var result = await _service.GetLinksAsync();

        //Assert
        Assert.Equal(result.Count, links.Count());
    }
    
    [Fact]
    public async Task GetAll_ThrowException_ReturnEmptyListOfLinks()
    {
        //Arrange
        _linkRepositoryMock.Setup(rep=> rep.GetAll()).Throws(new Exception());
        _service = new LinkService(_loggerMock.Object, _linkRepositoryMock.Object);
        
        //Act
        var result = await _service.GetLinksAsync();

        //Assert
        Assert.Empty(result);
    }
}