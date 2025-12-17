using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using MusicDBApp.Models;

namespace MusicDBApp.Areas.Artist.Controllers
{
    [Area("Artist")]
    public class ArtistController : Controller
    {
        public MusicDbContext ctx;
        public ArtistController(MusicDbContext ctx)
        {
            this.ctx = ctx;
        }
        public IActionResult Index()
        {
            // ⛔ Chặn nếu không phải Aritst
            var role = HttpContext.Session.GetString("Role");
            if (role != "Artist")
            {
                return RedirectToAction("Login", "Account", new { area = "" });
            }
            List<Song> songs = ctx.Songs
                .Include(s => s.Artist)
                .Include(s => s.Album)
                .ToList();
            return View(songs);
        }
       
    }
}
