using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.Rendering;
using Microsoft.EntityFrameworkCore;
using MusicDBApp.Models;

namespace MusicDBApp.Areas.Artist.Controllers
{
    [Area("Artist")]
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
            if (role != "Artist") return null;
            return HttpContext.Session.GetInt32("ArtistID");
        }
        //List
        public IActionResult List()
        {
            int? artistId = GetCurrentArtistId();
            if (artistId == null) return RedirectToAction("Login", "Account", new { area = "" });

            var artist = ctx.Artists.FirstOrDefault(a => a.ArtistId == artistId);
            if (artist == null) return RedirectToAction("Login");
            int realUserId = artist.UserId;

            List<Playlist> playlists = ctx.Playlists
                .Where(x => x.UserId == realUserId)
                .Include(p => p.Songs)
                .OrderByDescending(x => x.CreateDate)
                .ToList();

            return View(playlists);
        }

        //Detail Page
        public IActionResult Details(int id)
        {
            int? artistId = GetCurrentArtistId();
            if (artistId == null) return RedirectToAction("Login", "Account", new { area = "" });

            var artist = ctx.Artists.FirstOrDefault(a => a.ArtistId == artistId);
            if (artist == null) return RedirectToAction("Login");
            int realUserId = artist.UserId;

            Playlist p = ctx.Playlists
                .Include(x => x.Songs)
                .ThenInclude(s => s.Artist)
                .FirstOrDefault(x => x.PlaylistId == id && x.UserId == realUserId); 

            if (p == null) return NotFound();

            ViewBag.AllSongs = new SelectList(ctx.Songs.ToList(), "SongId", "SongName");
            return View(p);
        }

        //Add Song into Playlist
        [HttpGet]
        public IActionResult SelectSong(int playlistId)
        {
            int? artistId = GetCurrentArtistId();
            if (artistId == null) return RedirectToAction("Login", "Account", new { area = "" });

            var artist = ctx.Artists.FirstOrDefault(a => a.ArtistId == artistId);
            if (artist == null) return RedirectToAction("Login");
            int realUserId = artist.UserId;

            var allSongs = ctx.Songs.Include(s => s.Artist).ToList();
            var existingSongIds = ctx.Playlists
                .Where(p => p.PlaylistId == playlistId && p.UserId == realUserId) 
                .SelectMany(p => p.Songs.Select(s => s.SongId))
                .ToList();

            ViewBag.TargetPlaylistId = playlistId;
            ViewBag.ExistingSongIds = existingSongIds;

            return View(allSongs);
        }

        [HttpPost]
        public IActionResult AddSongToPlaylist(int playlistId, int songId)
        {
            int? artistId = GetCurrentArtistId();
            if (artistId == null) return RedirectToAction("Login", "Account", new { area = "" });

            var artist = ctx.Artists.FirstOrDefault(a => a.ArtistId == artistId);
            if (artist == null) return RedirectToAction("Login");
            int realUserId = artist.UserId;

            var p = ctx.Playlists
                .Include(x => x.Songs)
                .FirstOrDefault(x => x.PlaylistId == playlistId && x.UserId == realUserId); 

            var s = ctx.Songs.FirstOrDefault(x => x.SongId == songId);

            if (p != null && s != null)
            {
                if (!p.Songs.Contains(s))
                {
                    p.Songs.Add(s);
                    ctx.SaveChanges();
            }
            }

            return RedirectToAction("Details", new { id = playlistId });
        }

        //Delete Song in Playlist
        [HttpGet]
        public IActionResult RemoveSong(int playlistId, int songId)
        {
            int? artistId = GetCurrentArtistId();
            if (artistId == null) return RedirectToAction("Login", "Account", new { area = "" });

            var artist = ctx.Artists.FirstOrDefault(a => a.ArtistId == artistId);
            if (artist == null) return RedirectToAction("Login");
            int realUserId = artist.UserId;

            var p = ctx.Playlists
                .Include(x => x.Songs)
                .FirstOrDefault(x => x.PlaylistId == playlistId && x.UserId == realUserId); 

            if (p != null)
            {
                var s = p.Songs.FirstOrDefault(x => x.SongId == songId);
                if (s != null)
                {
                    p.Songs.Remove(s);
                    ctx.SaveChanges();
            }
            }
            return RedirectToAction("Details", new { id = playlistId });
        }

        //Create Playlist
        [HttpGet]
        public IActionResult Create()
        {
            if (GetCurrentArtistId() == null)
            {
                return RedirectToAction("Login", "Account", new { area = "" });
            }
            return View(new Playlist());
        }

        // 
        [HttpPost]
        public IActionResult Create(Playlist model)
        {
            if (string.IsNullOrEmpty(model.PlaylistName))
            {
                return View(model);
            }

            int? artistId = GetCurrentArtistId();
            if (artistId == null) return RedirectToAction("Login", "Account", new { area = "" });

            var artist = ctx.Artists.FirstOrDefault(a => a.ArtistId == artistId);
            if (artist == null) return RedirectToAction("Login");
            int realUserId = artist.UserId;

            model.UserId = realUserId; 
            model.CreateDate = DateTime.Now;

            ctx.Playlists.Add(model);
            ctx.SaveChanges();

            return RedirectToAction("List");
        }

        //Delete Playlist
        [HttpGet]
        public IActionResult Delete(int id)
        {
            int? artistId = GetCurrentArtistId();
            if (artistId == null) return RedirectToAction("Login", "Account", new { area = "" });

            var artist = ctx.Artists.FirstOrDefault(a => a.ArtistId == artistId);
            if (artist == null) return RedirectToAction("Login");
            int realUserId = artist.UserId;

            var p = ctx.Playlists
                .Include(x => x.Songs)
                .FirstOrDefault(x => x.PlaylistId == id && x.UserId == realUserId); 
            return View(p);
        }

        [HttpPost]
        public IActionResult Delete(Playlist model)
        {
            int? artistId = GetCurrentArtistId();
            if (artistId == null) return RedirectToAction("Login", "Account", new { area = "" });

            var artist = ctx.Artists.FirstOrDefault(a => a.ArtistId == artistId);
            if (artist == null) return RedirectToAction("Login");
            int realUserId = artist.UserId;
            var p = ctx.Playlists
                .Include(x => x.Songs)
                .FirstOrDefault(x => x.PlaylistId == model.PlaylistId && x.UserId == realUserId); 

            if (p != null)
            {
                p.Songs.Clear();
                ctx.Playlists.Remove(p);
                ctx.SaveChanges();
            }

            return RedirectToAction("List");
        }
    }
}