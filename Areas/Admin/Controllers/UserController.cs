using System.Linq;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using MusicDBApp.Models;

namespace MusicDBApp.Areas.Admin.Controllers
{
    [Area("Admin")]
    public class UserController : Controller
    {
        public MusicDbContext ctx;
        public UserController(MusicDbContext ctx)
        {
            this.ctx = ctx;
        }
        public IActionResult List()
        {
            List<User> users = ctx.Users.
                   Include(u => u.Role).
                   Include(u => u.Artists)
                   .ToList();
            return View(users);
        }


        public IActionResult Delete(int id)
        {
            User c = ctx.Users.Where(u => u.UserId == id).FirstOrDefault();
            ctx.Users.Remove(c);
            ctx.SaveChanges();
            return RedirectToAction("List");
        }

        [HttpGet]
        public IActionResult AddNew()
        {
            User users = new User();
            return View(users);
        }
        [HttpPost]
        public IActionResult AddNew(User user)
        {
            ctx.Users.Add(user);
            ctx.SaveChanges();
            return RedirectToAction("List");
        }

        [HttpPost]
        public IActionResult Find()
        {
            string name = Request.Form["searchName"].ToString();
            List<User> users = ctx.Users
                .Where(u => u.UserName!.Contains(name))
                .Include(u => u.Role)
                .Include(u => u.Artists)
                .ToList();
            return View(users);
        }
        [HttpGet]
        public IActionResult Update(int id)
        {
            User c = ctx.Users.FirstOrDefault(x => x.UserId == id);
            if (c == null)
                return NotFound();
            return View(c);
        }
        [HttpPost]
        public IActionResult Update(User user)
        {
            User c = ctx.Users.Where(u => u.UserId == user.UserId).FirstOrDefault();
            if (c == null)
                return NotFound();
            c.UserId = user.UserId;
            c.UserName = user.UserName;
            c.Email = user.Email;
            c.FullName = user.FullName;
            c.Password = user.Password;
            c.RoleId = user.RoleId;
            ctx.SaveChanges();
            return RedirectToAction("List", "User", new { area = "Admin" });
        }


    }
}