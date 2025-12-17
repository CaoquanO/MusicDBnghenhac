using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using MusicDBApp.Models;

namespace MusicDBApp.Controllers
{
    public class AccountController : Controller
    {
        private readonly MusicDbContext ctx;

        public AccountController(MusicDbContext context)
        {
            ctx = context;
        }

        // GET: Login
        [HttpGet]
        public IActionResult Login()
        {
            return View();
        }

        // POST: Login
        [HttpPost]
        public IActionResult Login(string username, string password)
        {
            var user = ctx.Users
                .Include(u => u.Role)
                .FirstOrDefault(u => u.Email == username && u.Password == password);

            if (user == null)
            {
                ViewBag.Error = "Sai tài khoản hoặc mật khẩu!";
                return View();
            }

            // Lưu session cơ bản
            HttpContext.Session.SetString("Username", user.Email);
            HttpContext.Session.SetString("Role", user.Role.RoleId);

            // ⚡ LƯU ARTIST ID NẾU LÀ ARTIST
            if (user.Role.RoleId == "Artist")
            {
                var artist = ctx.Artists.FirstOrDefault(a => a.UserId == user.UserId);

                if (artist != null)
                {
                    HttpContext.Session.SetInt32("ArtistID", artist.ArtistId);
                }
            }

            // Chuyển hướng theo Role
            switch (user.Role.RoleId)
            {
                case "Admin":
                    return RedirectToAction("Index", "Admin", new { area = "Admin" });

                case "Artist":
                    return RedirectToAction("Index", "Artist", new { area = "Artist" });

                default:
                    return RedirectToAction("Index", "Home");
            }
        }

        // GET: Register
        [HttpGet]
        public IActionResult Register()
        {
            return View();
        }

        // POST: Register
        [HttpPost]
        public IActionResult Register(string username, string password, string email, string fullName, DateTime birthDate)
        {
            // TODO: xử lý lưu thông tin vào DB
            var check = ctx.Users.FirstOrDefault(u => u.UserName == username);
            if (check != null)
            {
                ViewBag.Error = "Tên đăng nhập đã tồn tại!";
                return View();
            }
            var user = new User
            {
                UserName = username,
                Email = email,
                FullName = fullName,
                Password = password,
                DateOfBirth = DateOnly.FromDateTime(birthDate),
                RoleId = "User" // Mặc định là Artist
            };
            ctx.Users.Add(user);
            ctx.SaveChanges();
            return RedirectToAction("Login");
        }
    }
}
