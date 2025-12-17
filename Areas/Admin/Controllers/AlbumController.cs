using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using MusicDBApp.Models;

namespace MusicDBApp.Areas.Admin.Controllers
{
    [Area("Admin")]
    public class AlbumController : Controller
    {
        public MusicDbContext ctx;
        public AlbumController(MusicDbContext ctx)
        {
            this.ctx = ctx;
        }
        public IActionResult List()
        {
            List<Album> albums = ctx.Albums.Include(u => u.Artist).ToList();
            return View(albums);
        }
    }
}
