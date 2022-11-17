using AutoFixture;
using LinkShortener.DAL.Interfaces;
using LinkShortener.DAL.Models;
using LinkShortener.DAL.Repositories;
using LinkShortener.DAL.ViewModels;
using LinkShortener.Service.Services;
using Microsoft.Extensions.DependencyInjection;
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
        var result = await _service.DeleteLinkAsync(someId);
        
        //Assert
        Assert.True(result);
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
        var result = await _service.DeleteLinkAsync(someId);
        
        //Assert
        Assert.False(result);
    }
    
    [Fact]
    public async Task Delete_ThrowException_ShouldReturnFalse()
    {
        //Arrange
        var someId = It.IsAny<int>();
        
        _linkRepositoryMock.Setup(rep=> rep.GetByIdAsync(someId)).Throws(new Exception());
        _service = new LinkService(_loggerMock.Object, _linkRepositoryMock.Object);
        
        //Act
        var result = await _service.DeleteLinkAsync(someId);
        
        //Assert
        Assert.False(result);
    }

    [Fact]
    public async Task Create_ThrowException_ShouldReturnFalse()
    {
        //Arrange
        var longUrlViewModel = _fixture.Create<LongUrlViewModel>();
        _linkRepositoryMock.Setup(rep => rep.GetAll()).Throws(new Exception());
        
        _service = new LinkService(_loggerMock.Object, _linkRepositoryMock.Object);
        
        //Act
        var result = await _service.CreateLinkAsync(longUrlViewModel);
        
        //Assert
        Assert.False(result);
    }
    
    [Fact]
    public async Task Create_LongUrlViewModelExistsLink_ShouldReturnTrue()
    {
        //Arrange
        const string longUrl = "https://longurl/";
        var longUrlViewModel = _fixture.Build<LongUrlViewModel>().Do(x => x.LongUrl = longUrl).Create();
        var link = _fixture.Build<Link>().Do(x => x.LongUrl = longUrl).Create();
        var links = _fixture.CreateMany<Link>().AsQueryable().Append(link).BuildMock();

        _linkRepositoryMock.Setup(rep => rep.GetAll()).Returns(links);
        _service = new LinkService(_loggerMock.Object, _linkRepositoryMock.Object);
        
        //Act
        var result = await _service.CreateLinkAsync(longUrlViewModel);
        
        //Assert
        _linkRepositoryMock.Verify(rep => rep.CreateAsync(link), Times.Never);
        Assert.True(result);
    }
    [Fact]
    public async Task Create_LongUrlViewModelNotExistsLink_ShouldReturnTrue()
    {
        //Arrange
        var longUrlViewModel = _fixture.Build<LongUrlViewModel>().Create();
        var links = _fixture.CreateMany<Link>().AsQueryable().BuildMock();

        _linkRepositoryMock.Setup(rep => rep.GetAll()).Returns(links);
        _linkRepositoryMock.Setup(rep=>rep.CreateAsync(It.IsAny<Link>()));
        
        _service = new LinkService(_loggerMock.Object, _linkRepositoryMock.Object);

        //Act
        var result = await _service.CreateLinkAsync(longUrlViewModel);
        
        //Assert
        _linkRepositoryMock.Verify(rep => rep.CreateAsync(It.IsAny<Link>()), Times.Once);
        Assert.True(result);
    }
    
}