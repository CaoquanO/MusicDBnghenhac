using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.Rendering;
using Microsoft.EntityFrameworkCore;
using MusicDBApp.Models;

namespace MusicDBApp.Areas.Artist.Controllers
{
    [Area("Artist")]
    public class PlaylistController : Controller
    {
        private readonly MusicDbContext _ctx;

        public PlaylistController(MusicDbContext ctx)
        {
            _ctx = ctx;
        }

        // =========================
        // HÀM DÙNG CHUNG
        // =========================
        private int? GetArtistId()
        {
            if (HttpContext.Session.GetString("Role") != "Artist")
                return null;

            return HttpContext.Session.GetInt32("ArtistID");
        }

        // =========================
        // LIST PLAYLIST
        // =========================
        public IActionResult List()
        {
            int? artistId = GetArtistId();
            if (artistId == null)
                return RedirectToAction("Login", "Account", new { area = "" });

            var playlists = _ctx.Playlists
                .Where(p => p.UserId == artistId)
                .Include(p => p.Songs)
                .OrderByDescending(p => p.CreateDate)
                .ToList();

            return View(playlists);
        }

        // =========================
        // DETAILS
        // =========================
        public IActionResult Details(int id)
        {
            int? artistId = GetArtistId();
            if (artistId == null)
                return RedirectToAction("Login", "Account", new { area = "" });

            var playlist = _ctx.Playlists
                .Include(p => p.Songs)
                .ThenInclude(s => s.Artist)
                .FirstOrDefault(p => p.PlaylistId == id && p.UserId == artistId);

            if (playlist == null)
                return NotFound();

            // Chỉ lấy bài hát của chính Artist
            ViewBag.AllSongs = new SelectList(
                _ctx.Songs.Where(s => s.ArtistId == artistId),
                "SongId",
                "SongName"
            );

            return View(playlist);
        }

        // =========================
        // CREATE PLAYLIST
        // =========================
        [HttpGet]
        public IActionResult Create()
        {
            if (GetArtistId() == null)
                return RedirectToAction("Login", "Account", new { area = "" });

            return View();
        }

        [HttpPost]
        public IActionResult Create(Playlist model)
        {
            int? artistId = GetArtistId();
            if (artistId == null)
                return RedirectToAction("Login", "Account", new { area = "" });

            if (string.IsNullOrWhiteSpace(model.PlaylistName))
            {
                ModelState.AddModelError("", "Tên playlist không được để trống");
                return View(model);
            }

            model.UserId = artistId.Value;
            model.CreateDate = DateTime.Now;

            _ctx.Playlists.Add(model);
            _ctx.SaveChanges();

            return RedirectToAction("List");
        }

        // =========================
        // ADD SONG TO PLAYLIST
        // =========================
        [HttpPost]
        public IActionResult AddSong(int playlistId, int songId)
        {
            int? artistId = GetArtistId();
            if (artistId == null)
                return RedirectToAction("Login", "Account", new { area = "" });

            var playlist = _ctx.Playlists
                .Include(p => p.Songs)
                .FirstOrDefault(p => p.PlaylistId == playlistId && p.UserId == artistId);

            if (playlist == null)
                return NotFound();

            var song = _ctx.Songs.FirstOrDefault(s => s.SongId == songId && s.ArtistId == artistId);
            if (song == null)
                return NotFound();

            if (!playlist.Songs.Any(s => s.SongId == songId))
            {
                playlist.Songs.Add(song);
                _ctx.SaveChanges();
            }

            return RedirectToAction("Details", new { id = playlistId });
        }

        // =========================
        // REMOVE SONG
        // =========================
        [HttpGet]
        public IActionResult RemoveSong(int playlistId, int songId)
        {
            int? artistId = GetArtistId();
            if (artistId == null)
                return RedirectToAction("Login", "Account", new { area = "" });

            var playlist = _ctx.Playlists
                .Include(p => p.Songs)
                .FirstOrDefault(p => p.PlaylistId == playlistId && p.UserId == artistId);

            if (playlist == null)
                return NotFound();

            var song = playlist.Songs.FirstOrDefault(s => s.SongId == songId);
            if (song != null)
            {
                playlist.Songs.Remove(song);
                _ctx.SaveChanges();
            }

            return RedirectToAction("Details", new { id = playlistId });
        }

        // =========================
        // DELETE PLAYLIST
        // =========================
        [HttpGet]
        public IActionResult Delete(int id)
        {
            int? artistId = GetArtistId();
            if (artistId == null)
                return RedirectToAction("Login", "Account", new { area = "" });

            var playlist = _ctx.Playlists
                .FirstOrDefault(p => p.PlaylistId == id && p.UserId == artistId);

            if (playlist == null)
                return NotFound();

            return View(playlist);
        }

        [HttpPost]
        public IActionResult DeleteConfirmed(int playlistId)
        {
            int? artistId = GetArtistId();
            if (artistId == null)
                return RedirectToAction("Login", "Account", new { area = "" });

            var playlist = _ctx.Playlists
                .Include(p => p.Songs)
                .FirstOrDefault(p => p.PlaylistId == playlistId && p.UserId == artistId);

            if (playlist != null)
            {
                playlist.Songs.Clear();
                _ctx.Playlists.Remove(playlist);
                _ctx.SaveChanges();
            }

            return RedirectToAction("List");
        }
    }
}
