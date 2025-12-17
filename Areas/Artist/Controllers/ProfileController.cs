using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using MusicDBApp.Models;

namespace MusicDBApp.Areas.Artist.Controllers
{
    [Area("Artist")]
    public class ProfileController : Controller
    {
        private readonly MusicDbContext ctx;

        public ProfileController(MusicDbContext ctx)
        {
            this.ctx = ctx;
        }

        public IActionResult Index()
        {
            var username = HttpContext.Session.GetString("Username");
            if (username == null)
                return RedirectToAction("Login", "Account");

            // Lấy user theo session
            var user = ctx.Users
                .Include(u => u.Role)
                .FirstOrDefault(u => u.Email == username);

            if (user == null)
                return RedirectToAction("Login", "Account");

            return View(user);
        }
    }
}
