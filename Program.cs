using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Configuration;
using MusicDBApp.Models;

var builder = WebApplication.CreateBuilder(args);

// Add services to the container.
builder.Services.AddControllersWithViews();
builder.Services.AddSession();
builder.Services.AddDbContext<MusicDbContext>(
 options => options.UseSqlServer(builder.Configuration.GetConnectionString("MyDB")));

var app = builder.Build();

// Configure the HTTP request pipeline.
if (!app.Environment.IsDevelopment())
{
    app.UseExceptionHandler("/Home/Error");
    // The default HSTS value is 30 days. You may want to change this for production scenarios, see https://aka.ms/aspnetcore-hsts.
    app.UseHsts();
}

app.UseHttpsRedirection();
app.UseRouting();
app.UseSession();
app.UseStaticFiles();
app.UseAuthorization();

app.MapStaticAssets();
app.MapAreaControllerRoute(
    name: "admin",
    areaName: "Admin",
    pattern: "Admin/{controller=Admin}/{action=Index}/{id?}"
);
app.MapAreaControllerRoute(
    name: "artist",
    areaName: "Artist",
    pattern: "Artist/{controller=Artist}/{action=Index}/{id?}"
);

app.MapControllerRoute(
    name: "default",
    pattern: "{controller=Account}/{action=Login}/{id?}")
    .WithStaticAssets();
app.Run();
