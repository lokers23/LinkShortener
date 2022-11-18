using AutoFixture;
using LinkShortener.DAL;
using LinkShortener.DAL.Interfaces;
using LinkShortener.DAL.Repositories;
using LinkShortener.Domain.Models;
using LinkShortener.Domain.ViewModels;
using LinkShortener.Service.Interfaces;
using LinkShortener.Service.Services;
using Microsoft.AspNetCore.Hosting;
using Microsoft.AspNetCore.Mvc.Testing;
using Microsoft.AspNetCore.TestHost;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Logging;
using Moq;



namespace LinkShortener.Service.IntegrationTests
{
    public class LinkServiceTets
    {
        private readonly Mock<ILogger<LinkService>> _loggerMock;
        private readonly Fixture _fixture;

        public LinkServiceTets()
        {
            _fixture = new Fixture();
            _loggerMock = new Mock<ILogger<LinkService>>();
        }

        [Fact]
        public async Task DeleteLinksAsync_ShouldReturnTrue()
        {
            //Arrange
            var webHost = new WebApplicationFactory<Program>()
            .WithWebHostBuilder(builer =>
            {
                builer.ConfigureTestServices(services =>
                {
                    var dbContextShortLink = services.SingleOrDefault(d =>
                        d.ServiceType == typeof(DbContextOptions<ShortLinkDbContext>));

                    services.Remove(dbContextShortLink);

                    services.AddDbContext<ShortLinkDbContext>(options =>
                    {
                        options.UseInMemoryDatabase(Guid.NewGuid().ToString());
                    });
                });
            });

            var context = webHost.Services.CreateScope().ServiceProvider.GetService<ShortLinkDbContext>();
            var links = new List<Link>() { new Link() { Id = 1 }, new Link() { Id = 2 }, new Link() { Id = 3 } };
            await context.AddRangeAsync(links);
            await context.SaveChangesAsync();
         
            var rep = new LinkRepository(context);
            var _linkService = new LinkService(_loggerMock.Object, rep);

            //Act
            var response = await _linkService.DeleteLinkAsync(1);

            //Assert
            Assert.True(response.Data);
        }
        
        [Fact]
        public async Task DeleteLinksAsync_ShouldReturnFalse()
        {
            //Arrange
            var webHost = new WebApplicationFactory<Program>()
            .WithWebHostBuilder(builer =>
            {
                builer.ConfigureTestServices(services =>
                {
                    var dbContextShortLink = services.SingleOrDefault(d =>
                        d.ServiceType == typeof(DbContextOptions<ShortLinkDbContext>));

                    services.Remove(dbContextShortLink);

                    services.AddDbContext<ShortLinkDbContext>(options =>
                    {
                        options.UseInMemoryDatabase(Guid.NewGuid().ToString());
                    });
                });
            });

            var context = webHost.Services.CreateScope().ServiceProvider.GetService<ShortLinkDbContext>();
            var links = new List<Link>() { new Link() { Id = 1 }, new Link() { Id = 2 }, new Link() { Id = 3 } };
            await context.AddRangeAsync(links);
            await context.SaveChangesAsync();
         
            var rep = new LinkRepository(context);
            var _linkService = new LinkService(_loggerMock.Object, rep);

            //Act
            var response = await _linkService.DeleteLinkAsync(4);

            //Assert
            Assert.False(response.Data);
        }
        
        [Fact]
        public async Task GetLinksAsync_ShouldReturnLinks()
        {
            //Arrange
            var webHost = new WebApplicationFactory<Program>()
            .WithWebHostBuilder(builer =>
            {
                builer.ConfigureTestServices(services =>
                {
                    var dbContextShortLink = services.SingleOrDefault(d =>
                        d.ServiceType == typeof(DbContextOptions<ShortLinkDbContext>));

                    services.Remove(dbContextShortLink);

                    services.AddDbContext<ShortLinkDbContext>(options =>
                    {
                        options.UseInMemoryDatabase(Guid.NewGuid().ToString());
                    });
                });
            });

            var context = webHost.Services.CreateScope().ServiceProvider.GetService<ShortLinkDbContext>();
            var links = new List<Link>() { new Link() { Id = 1 }, new Link() { Id = 2 }, new Link() { Id = 3 } };
            await context.AddRangeAsync(links);
            await context.SaveChangesAsync();
         
            var rep = new LinkRepository(context);
            var _linkService = new LinkService(_loggerMock.Object, rep);

            //Act
            var response = await _linkService.GetLinksAsync();

            //Assert
            for (int i = 0; i < response.Data.Count; i++)
            {
                Assert.Equal(response.Data[i], links[i]);
            }
        }

        [Fact]
        public async Task GetLinkByIdAsync_ShouldReturnLink()
        {
            //Arrange
            var webHost = new WebApplicationFactory<Program>()
            .WithWebHostBuilder(builer =>
            {
                builer.ConfigureTestServices(services =>
                {
                    var dbContextShortLink = services.SingleOrDefault(d =>
                        d.ServiceType == typeof(DbContextOptions<ShortLinkDbContext>));

                    services.Remove(dbContextShortLink);

                    services.AddDbContext<ShortLinkDbContext>(options =>
                    {
                        options.UseInMemoryDatabase(Guid.NewGuid().ToString());
                    });
                });
            });

            var context = webHost.Services.CreateScope().ServiceProvider.GetService<ShortLinkDbContext>();
            var expected = new Link() { Id = 1 };
            var links = new List<Link>() { expected, new Link() { Id = 2 }, new Link() { Id = 3 } };
            await context.AddRangeAsync(links);
            await context.SaveChangesAsync();

            var rep = new LinkRepository(context);
            var _linkService = new LinkService(_loggerMock.Object, rep);

            //Act
            var response = await _linkService.GetLinkByIdAsync(1);

            //Assert
            Assert.Equal(response.Data, expected);
        }

        [Fact]
        public async Task GetLinkByIdAsync_ShouldReturnNull()
        {
            //Arrange
            var webHost = new WebApplicationFactory<Program>()
            .WithWebHostBuilder(builer =>
            {
                builer.ConfigureTestServices(services =>
                {
                    var dbContextShortLink = services.SingleOrDefault(d =>
                        d.ServiceType == typeof(DbContextOptions<ShortLinkDbContext>));

                    services.Remove(dbContextShortLink);

                    services.AddDbContext<ShortLinkDbContext>(options =>
                    {
                        options.UseInMemoryDatabase(Guid.NewGuid().ToString());
                    });
                });
            });

            var context = webHost.Services.CreateScope().ServiceProvider.GetService<ShortLinkDbContext>();
            var expected = new Link() { Id = 1 };
            var links = new List<Link>() { expected, new Link() { Id = 2 }, new Link() { Id = 3 } };
            await context.AddRangeAsync(links);
            await context.SaveChangesAsync();

            var rep = new LinkRepository(context);
            var _linkService = new LinkService(_loggerMock.Object, rep);

            //Act
            var response = await _linkService.GetLinkByIdAsync(5);

            //Assert
            Assert.Null(response.Data);
        }


        [Fact]
        public async Task CreateLinkAsync_ValidModel_ShouldReturnTrue()
        {
            //Arrange
            var linkViewModel = _fixture.Build<LongUrlViewModel>()
                .With(l => l.Id, 0)
                .Create();

            var webHost = new WebApplicationFactory<Program>()
            .WithWebHostBuilder(builer =>
            {
                builer.ConfigureTestServices(services =>
                {
                    var dbContextShortLink = services.SingleOrDefault(d =>
                        d.ServiceType == typeof(DbContextOptions<ShortLinkDbContext>));

                    services.Remove(dbContextShortLink);

                    services.AddDbContext<ShortLinkDbContext>(options =>
                    {
                        options.UseInMemoryDatabase(Guid.NewGuid().ToString());
                    });
                });
            });

            var context = webHost.Services.CreateScope().ServiceProvider.GetService<ShortLinkDbContext>();
            var links = new List<Link>() { new Link() { Id = 1 }, new Link() { Id = 2 }, new Link() { Id = 3 } };
            await context.AddRangeAsync(links);
            await context.SaveChangesAsync();

            var rep = new LinkRepository(context);
            var _linkService = new LinkService(_loggerMock.Object, rep);

            //Act
            var response = await _linkService.CreateLinkAsync(linkViewModel);

            //Assert
            Assert.True(response.Data);
            Assert.True(context.Links.Count() == links.Count + 1);
        }

        [Fact]
        public async Task CreateLinkAsync_InvalidModel_ShouldReturnFalse()
        {
            //Arrange
            var linkViewModel = _fixture.Build<LongUrlViewModel>()
                .With(l => l.Id, 2)
                .Create();

            var webHost = new WebApplicationFactory<Program>()
            .WithWebHostBuilder(builer =>
            {
                builer.ConfigureTestServices(services =>
                {
                    var dbContextShortLink = services.SingleOrDefault(d =>
                        d.ServiceType == typeof(DbContextOptions<ShortLinkDbContext>));

                    services.Remove(dbContextShortLink);

                    services.AddDbContext<ShortLinkDbContext>(options =>
                    {
                        options.UseInMemoryDatabase(Guid.NewGuid().ToString());
                    });
                });
            });

            var context = webHost.Services.CreateScope().ServiceProvider.GetService<ShortLinkDbContext>();
            var links = new List<Link>() { new Link() { Id = 1 }, new Link() { Id = 2 }, new Link() { Id = 3 } };
            await context.AddRangeAsync(links);
            await context.SaveChangesAsync();

            var repository = new LinkRepository(context);
            var _linkService = new LinkService(_loggerMock.Object, repository);

            //Act
            var response = await _linkService.CreateLinkAsync(linkViewModel);

            //Assert
            Assert.False(response.Data);
            Assert.True(context.Links.Count() == links.Count);
        }

        [Fact]
        public async Task UpdateLinkAsync_ValidModel_ShouldReturnTrue()
        {
            //Arrange
            var ValidId = 5;
            var linkViewModel = _fixture.Build<LongUrlViewModel>()
                .With(l => l.Id, ValidId)
                .Create();

            var webHost = new WebApplicationFactory<Program>()
            .WithWebHostBuilder(builer =>
            {
                builer.ConfigureTestServices(services =>
                {
                    var dbContextShortLink = services.SingleOrDefault(d =>
                        d.ServiceType == typeof(DbContextOptions<ShortLinkDbContext>));

                    services.Remove(dbContextShortLink);

                    services.AddDbContext<ShortLinkDbContext>(options =>
                    {
                        options.UseInMemoryDatabase(Guid.NewGuid().ToString());
                    });
                });
            });

            var context = webHost.Services.CreateScope().ServiceProvider.GetService<ShortLinkDbContext>();
            var links = new List<Link>() { new Link() { Id = 1 }, new Link() { Id = 2 }, new Link() { Id = ValidId } };
            await context.AddRangeAsync(links);
            await context.SaveChangesAsync();

            var repository = new LinkRepository(context);
            var _linkService = new LinkService(_loggerMock.Object, repository);

            //Act
            var response = await _linkService.UpdateLinkAsync(linkViewModel);

            //Assert
            Assert.True(response.Data);
            var link = context.Links.FirstOrDefault(x => x.Id == ValidId);
            Assert.NotNull(link);

            Assert.Equal(link?.LongUrl, linkViewModel.LongUrl);
            Assert.Equal(link?.DateCreate, linkViewModel.DateCreate);
            Assert.Equal(link?.CountClick, linkViewModel.CountClick);
        }

        [Fact]
        public async Task UpdateLinkAsync_ValidModel_ShouldReturnFalse()
        {
            //Arrange
            var ValidId = 999;
            var linkViewModel = _fixture.Build<LongUrlViewModel>()
                .With(l => l.Id, ValidId)
                .Create();

            var webHost = new WebApplicationFactory<Program>()
            .WithWebHostBuilder(builer =>
            {
                builer.ConfigureTestServices(services =>
                {
                    var dbContextShortLink = services.SingleOrDefault(d =>
                        d.ServiceType == typeof(DbContextOptions<ShortLinkDbContext>));

                    services.Remove(dbContextShortLink);

                    services.AddDbContext<ShortLinkDbContext>(options =>
                    {
                        options.UseInMemoryDatabase(Guid.NewGuid().ToString());
                    });
                });
            });

            var context = webHost.Services.CreateScope().ServiceProvider.GetService<ShortLinkDbContext>();
            var links = new List<Link>() { new Link() { Id = 1 }, new Link() { Id = 2 }, new Link() { Id = 3 } };
            await context.AddRangeAsync(links);
            await context.SaveChangesAsync();

            var repository = new LinkRepository(context);
            var _linkService = new LinkService(_loggerMock.Object, repository);

            //Act
            var response = await _linkService.UpdateLinkAsync(linkViewModel);

            //Assert
            Assert.False(response.Data);
            var link = context.Links.FirstOrDefault(x => x.Id == ValidId);
            Assert.Null(link);
        }
    }
}
