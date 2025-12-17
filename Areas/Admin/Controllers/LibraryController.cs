using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using MusicDBApp.Models;

namespace MusicDBApp.Areas.Admin.Controllers
{
    [Area("Admin")]
    public class LibraryController : Controller
    {
        public MusicDbContext ctx;
        public LibraryController(MusicDbContext ctx)
        {
            this.ctx = ctx;
        }
        public IActionResult List()
        {
            List<Album> albums = ctx.Albums
         .Include(a => a.Artist)
         .Include(a => a.Songs)
         .ToList();
            return View(albums);
        }
    }
}
