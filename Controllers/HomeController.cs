using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using MusicDB.Models;      // Namespace chứa dbContext của bạn (check lại nếu khác)
using MusicDBApp.Models;   // Namespace chứa Model Song, Album
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
        var songs = ctx.Songs
            .Include(a => a.Artist)
            .Include(a => a.Album)
            .Include(a => a.Users)
            .OrderBy(x => Guid.NewGuid()) 
            .Take(50) 
            .ToList();

        return View(songs);
    }
    [HttpGet]
    public IActionResult AlbumDetails(int id)
    {
        Album? album = ctx.Albums
            .Include(x => x.Songs)
            .ThenInclude(s => s.Artist) 
            .Include(x => x.Artist)    
            .FirstOrDefault(x => x.AlbumId == id);

        if (album == null)
        {
            return NotFound();
        }
        return View(album);
    }

    [HttpGet]
    public IActionResult AllAlbums()
    {

        List<Album> albums = ctx.Albums
            .Include(p => p.Songs)
            .Include(p => p.Artist) 
            .OrderByDescending(x => x.ReleaseDate) 
            .ToList();

        return View(albums);
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