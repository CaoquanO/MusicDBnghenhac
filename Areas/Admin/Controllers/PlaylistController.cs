using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.Rendering;
using Microsoft.EntityFrameworkCore;
using MusicDBApp.Models;

namespace MusicDBApp.Areas.Admin.Controllers
{
    [Area("Admin")]
    public class PlaylistController : Controller
    {
        public MusicDbContext ctx;
        public PlaylistController(MusicDbContext ctx)
        {
            this.ctx = ctx;
        }
        private int? GetCurrentArtistId()
        {
            var role = HttpContext.Session.GetString("Role");
            if (role != "") return null;

            // Lấy ID
            return HttpContext.Session.GetInt32("ArtistID");
        }

        // LIST
        public IActionResult List()
        {

            // Lọc dữ liệu: Chỉ lấy Playlist có ArtistId trùng với người đang đăng nhập
            List<Playlist> playlists = ctx.Playlists
                .Include(p => p.Songs)
                .OrderByDescending(x => x.CreateDate)
                .ToList();

            return View(playlists);
        }

        // DETAILS
        public IActionResult Details(int id)
        {
            int? artistId = GetCurrentArtistId();
            if (artistId == null) return RedirectToAction("Login", "Account", new { area = "" });

            List<Playlist> p = ctx.Playlists
                        .Where(x => x.PlaylistId == id && x.UserId == artistId)
                        .Include(x => x.Songs)
                        .ThenInclude(x => x.Artist)
                        .Include(x => x.Songs)
                        .ThenInclude(x => x.Album)
                        .ToList(); 

            if (p == null) return NotFound();

            ViewBag.AllSongs = new SelectList(ctx.Songs.ToList(), "SongId", "SongName");
            return View(p);
        }
        // REMOVE SONG
        [HttpGet]
        public IActionResult RemoveSong(int playlistId, int songId)
        {
            int? artistId = GetCurrentArtistId();
            if (artistId == null) return RedirectToAction("Login", "Account", new { area = "" });

            var p = ctx.Playlists
                        .Include(x => x.Songs)
                        .FirstOrDefault(x => x.PlaylistId == playlistId && x.UserId == artistId);

            if (p != null)
            {
                // Tìm bài hát trong playlist 
                var s = p.Songs.FirstOrDefault(x => x.SongId == songId);
                if (s != null)
                {
                    p.Songs.Remove(s);
                    ctx.SaveChanges();
                }
            }
            return RedirectToAction("Details", new { id = playlistId });
        }
        [HttpGet]
        public IActionResult Create()
        {
            if (HttpContext.Session.GetInt32("ArtistID") == null)
            {
                return RedirectToAction("Login", "Account", new { area = "" });
            }
            return View(new Playlist());
        }


        [HttpPost]
        public IActionResult Create(Playlist model)
        {
            if (string.IsNullOrEmpty(model.PlaylistName))
            {
                return View(model);
            }

            // Lấy ID người dùng 
            int? artistId = HttpContext.Session.GetInt32("ArtistID");
            if (artistId == null) return RedirectToAction("Login", "Account", new { area = "" });

            model.UserId = artistId.Value;
            model.CreateDate = DateTime.Now;
            ctx.Playlists.Add(model);
            ctx.SaveChanges();

            return RedirectToAction("List");
        }
    }
}