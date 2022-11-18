using AutoFixture;
using LinkShortener.DAL.Interfaces;
using LinkShortener.Domain.Models;
using LinkShortener.DAL.Repositories;
using LinkShortener.Domain.ViewModels;
using LinkShortener.Service.Services;
using Microsoft.Extensions.Logging;
using Moq;
using MockQueryable.Moq;
using LinkShortener.Domain.Response;

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
    public async Task GetLinkByIdAsync_ShouldCallGetLinkById()
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
        var response = await _service.GetLinkByIdAsync(link.Id);
        
        //Assert
        Assert.IsType<Link>(response.Data);
    }
    
    [Fact]
    public async Task GetById_ThrowException_ReturnNull()
    {
        //Arrange
        var link = _fixture.Create<Link>();

        _linkRepositoryMock.Setup(rep => rep.GetByIdAsync(link.Id)).ThrowsAsync(new Exception());
        _service = new LinkService(_loggerMock.Object, _linkRepositoryMock.Object);
        
        //Act
        var response = await _service.GetLinkByIdAsync(link.Id);
        
        //Assert
        Assert.Null(response.Data);
    }

    [Fact]
    public async Task GetAll_ReturnLinks()
    {
        //Arrange
        var links = _fixture.CreateMany<Link>().AsQueryable().BuildMock();
        
        _linkRepositoryMock.Setup(rep => rep.GetAll()).Returns(links);
        _service = new LinkService(_loggerMock.Object, _linkRepositoryMock.Object);
        
        //Act
        var response = await _service.GetLinksAsync();

        //Assert
        Assert.Equal(response.Data?.Count, links.Count());
    }
    
    [Fact]
    public async Task GetAll_ThrowException_ReturnEmptyListOfLinks()
    {
        //Arrange
        _linkRepositoryMock.Setup(rep=> rep.GetAll()).Throws(new Exception());
        _service = new LinkService(_loggerMock.Object, _linkRepositoryMock.Object);
        
        //Act
        var response = await _service.GetLinksAsync();

        //Assert
        Assert.Empty(response.Data);
    }

    [Fact]
    public async Task Delete_SomeIdExistsLink_ShouldReturnTrue()
    {
        //Arrange
        var link = _fixture.Create<Link>();
        var someId = It.Is<int>(x => x >= 0);
        
        _linkRepositoryMock.Setup(rep=> rep.GetByIdAsync(someId)).ReturnsAsync(link);
        _linkRepositoryMock.Setup(rep=> rep.DeleteAsync(link));
        _service = new LinkService(_loggerMock.Object, _linkRepositoryMock.Object);
        
        //Act
        var response = await _service.DeleteLinkAsync(someId);
        
        //Assert
        Assert.True(response.Data);
    }
    
    [Fact]
    public async Task Delete_SomeIdNotExistsLink_ShouldReturnFalse()
    {
        //Arrange
        Link? link = null;
        var someId = It.IsAny<int>();
        
        _linkRepositoryMock.Setup(rep=> rep.GetByIdAsync(someId)).ReturnsAsync(link);
        _service = new LinkService(_loggerMock.Object, _linkRepositoryMock.Object);
        
        //Act
        var response = await _service.DeleteLinkAsync(someId);
        
        //Assert
        Assert.False(response.Data);
    }
    
    [Fact]
    public async Task Delete_ThrowException_ShouldReturnFalse()
    {
        //Arrange
        var someId = It.IsAny<int>();
        
        _linkRepositoryMock.Setup(rep=> rep.GetByIdAsync(someId)).Throws(new Exception());
        _service = new LinkService(_loggerMock.Object, _linkRepositoryMock.Object);
        
        //Act
        var response = await _service.DeleteLinkAsync(someId);
        
        //Assert
        Assert.False(response.Data);
    }

    [Fact]
    public async Task Create_ThrowException_ShouldReturnFalse()
    {
        //Arrange
        var longUrlViewModel = _fixture.Create<LongUrlViewModel>();
        _linkRepositoryMock.Setup(rep => rep.GetAll()).Throws(new Exception());
        
        _service = new LinkService(_loggerMock.Object, _linkRepositoryMock.Object);
        
        //Act
        var response = await _service.CreateLinkAsync(longUrlViewModel);
        
        //Assert
        Assert.False(response.Data);
    }
    
    [Fact]
    public async Task Create_LongUrlViewModelExistsLink_ShouldReturnTrue()
    {
        //Arrange
        const string longUrl = "https://longurl/";
        var longUrlViewModel = _fixture.Build<LongUrlViewModel>()
            .With(x => x.LongUrl, longUrl)
            .With(x => x.Id, 0)
            .Create();

        var link = _fixture.Build<Link>()
            .With(x => x.LongUrl, longUrl)
            .Create();

        var links = _fixture.CreateMany<Link>()
            .AsQueryable()
            .Append(link)
            .BuildMock();

        _linkRepositoryMock.Setup(rep => rep.GetAll()).Returns(links);
        _service = new LinkService(_loggerMock.Object, _linkRepositoryMock.Object);
        
        //Act
        var response = await _service.CreateLinkAsync(longUrlViewModel);
        
        //Assert
        _linkRepositoryMock.Verify(rep => rep.CreateAsync(It.IsAny<Link>()), Times.Never);
        Assert.True(response.Data);
    }

    [Fact]
    public async Task Create_LongUrlViewModelNotExistsLink_ShouldReturnTrue()
    {
        //Arrange
        var longUrlViewModel = _fixture.Build<LongUrlViewModel>()
            .With(x => x.Id, 0)
            .Create();

        var links = _fixture.CreateMany<Link>()
            .AsQueryable()
            .BuildMock();

        _linkRepositoryMock.Setup(rep => rep.GetAll()).Returns(links);
        _linkRepositoryMock.Setup(rep=>rep.CreateAsync(It.IsAny<Link>()));
        
        _service = new LinkService(_loggerMock.Object, _linkRepositoryMock.Object);

        //Act
        var response = await _service.CreateLinkAsync(longUrlViewModel);
        
        //Assert
        _linkRepositoryMock.Verify(rep => rep.CreateAsync(It.IsAny<Link>()), Times.Once);
        Assert.True(response.Data);
    }
    
}