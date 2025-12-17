using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using MusicDB.Models;
using MusicDBApp.Models;
using System.Diagnostics;

namespace MusicDBApp.Controllers;

public class HomeController : Controller
{
    private readonly ILogger<HomeController> _logger;
    public MusicDbContext ctx;
    public HomeController(ILogger<HomeController> logger, MusicDbContext ctx)
    {
        _logger = logger;
        this.ctx = ctx;
    }
   public IActionResult Index()
    {
        var song = ctx.Songs
            .Include(a => a.Artist)
            .Include(a => a.Album)
            .Include(a => a.Users)
            .ToList();
        return View(song);
    }
    public IActionResult Privacy()
    {
        return View();
    }

    [ResponseCache(Duration = 0, Location = ResponseCacheLocation.None, NoStore = true)]
    public IActionResult Error()
    {
        return View(new ErrorViewModel { RequestId = Activity.Current?.Id ?? HttpContext.TraceIdentifier });
    }
}
