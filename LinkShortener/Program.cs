using LinkShortener.DAL;
using LinkShortener.DAL.Interfaces;
using LinkShortener.DAL.Models;
using LinkShortener.DAL.Repositories;
using LinkShortener.Service;
using LinkShortener.Service.Services;
using Microsoft.EntityFrameworkCore;

var builder = WebApplication.CreateBuilder(args);

// Add services to the container.
builder.Services.AddControllersWithViews();
builder.Services.AddDbContext<ShortLinkDbContext>();

builder.Services.AddScoped<IRepository<Link>, LinkRepository>();
builder.Services.AddScoped<ILinkService, LinkService>();

var app = builder.Build();

using (var scope = app.Services.CreateScope())
{
    var dataContext = scope.ServiceProvider.GetRequiredService<ShortLinkDbContext>();
    dataContext.Database.Migrate();
}

// Configure the HTTP request pipeline.
if (!app.Environment.IsDevelopment())
{
    app.UseExceptionHandler("/Home/Error");
    // The default HSTS value is 30 days. You may want to change this for production scenarios, see https://aka.ms/aspnetcore-hsts.
    app.UseHsts();
}

app.UseHttpsRedirection();
app.UseStaticFiles();

app.UseRouting();

app.UseAuthorization();

app.UseEndpoints( _ =>
{
    app.MapControllerRoute(
        name: "default",
        pattern: "{controller=Link}/{action=Index}/{id?}");
    
    app.MapControllerRoute(
        name: "shortUrl",
        pattern: "{url}",
        defaults: new {controller = "Link", action = "RedirectToLongUrl"});
});

app.Run();
