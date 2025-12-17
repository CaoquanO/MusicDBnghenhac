using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using MusicDBApp.Models;

namespace MusicDBApp.Areas.Admin.Controllers
{
    [Area("Admin")]
    public class AdminController : Controller
    {
        public MusicDbContext ctx;
        public AdminController(MusicDbContext ctx)
        {
            this.ctx = ctx;
        }
        public IActionResult Index()
        {
            // Chặn nếu không phải Admin
            var role = HttpContext.Session.GetString("Role");
            if (role != "Admin")
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
