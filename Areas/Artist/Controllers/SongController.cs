using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using MusicDBApp.Models;

namespace MusicDBApp.Areas.Artist.Controllers
{
    [Area("Artist")]
    public class SongController : Controller
    {
        public MusicDbContext ctx;
        public SongController(MusicDbContext ctx)
        {
            this.ctx = ctx;
        }
        public IActionResult List()
        {
            var role = HttpContext.Session.GetString("Role");
            if (role != "Artist")
                return RedirectToAction("Login", "Account", new { area = "" });

            int? artistId = HttpContext.Session.GetInt32("ArtistID");
            if (artistId == null)
                return RedirectToAction("Login", "Account", new { area = "" });

            var songs = ctx.Songs
                .Where(s => s.ArtistId == artistId)
                .Include(s => s.Album)
                .ToList();

            return View(songs);
        }
        [HttpPost]
        public IActionResult Find(string searchName)
        {
            var role = HttpContext.Session.GetString("Role");
            if (role != "Artist")
                return RedirectToAction("Login", "Account", new { area = "" });

            int? artistId = HttpContext.Session.GetInt32("ArtistID");
            if (artistId == null)
                return RedirectToAction("Login", "Account", new { area = "" });

            var songs = ctx.Songs
                .Where(s => s.ArtistId == artistId &&
                            s.SongName.Contains(searchName))
                .Include(s => s.Album)
                .ToList();

            return View("List", songs);   // ⭐ QUAN TRỌNG
        }
        [HttpGet]
        public IActionResult AddNew()
        {
            var role = HttpContext.Session.GetString("Role");
            if (role != "Artist")
                return RedirectToAction("Login", "Account", new { area = "" });

            int? artistId = HttpContext.Session.GetInt32("ArtistID");
            if (artistId == null)
                return RedirectToAction("Login", "Account", new { area = "" });

            ViewBag.Albums = ctx.Albums
                                .Where(a => a.ArtistId == artistId)
                                .ToList();

            return View();
        }
        [HttpPost]
        public IActionResult AddNew(Song song, IFormFile uploadFile, IFormFile uploadImage)
        {
            var role = HttpContext.Session.GetString("Role");
            if (role != "Artist")
                return RedirectToAction("Login", "Account", new { area = "" });

            int? artistId = HttpContext.Session.GetInt32("ArtistID");
            if (artistId == null)
                return RedirectToAction("Login", "Account", new { area = "" });

            song.ArtistId = artistId.Value;

            /* ====== UPLOAD FILE NHẠC ====== */
            if (uploadFile != null && uploadFile.Length > 0)
            {
                string songFolder = Path.Combine(Directory.GetCurrentDirectory(), "wwwroot/song");
                Directory.CreateDirectory(songFolder);

                string fileName = Guid.NewGuid() + Path.GetExtension(uploadFile.FileName);
                string filePath = Path.Combine(songFolder, fileName);

                using (var stream = new FileStream(filePath, FileMode.Create))
                {
                    uploadFile.CopyTo(stream);
                }

                song.FilePath = fileName;
            }

            /* ====== UPLOAD ẢNH ====== */
            if (uploadImage != null && uploadImage.Length > 0)
            {
                string imgFolder = Path.Combine(Directory.GetCurrentDirectory(), "wwwroot/songImage");
                Directory.CreateDirectory(imgFolder);

                string imgName = Guid.NewGuid() + Path.GetExtension(uploadImage.FileName);
                string imgPath = Path.Combine(imgFolder, imgName);

                using (var stream = new FileStream(imgPath, FileMode.Create))
                {
                    uploadImage.CopyTo(stream);
                }

                song.SongImage = imgName;
            }

            ctx.Songs.Add(song);
            ctx.SaveChanges();

            return RedirectToAction("List");
        }
        [HttpGet]
        public IActionResult Update(int id)
        {
            var role = HttpContext.Session.GetString("Role");
            if (role != "Artist")
                return RedirectToAction("Login", "Account", new { area = "" });

            int? artistId = HttpContext.Session.GetInt32("ArtistID");

            Song song = ctx.Songs
                .Include(s => s.Album)
                .FirstOrDefault(s => s.SongId == id && s.ArtistId == artistId);

            if (song == null)
                return NotFound();

            ViewBag.Albums = ctx.Albums
                .Where(a => a.ArtistId == artistId)
                .ToList();

            return View(song);
        }

        [HttpPost]
        public IActionResult Update(Song song, IFormFile uploadFile, IFormFile uploadImage)
        {
            var role = HttpContext.Session.GetString("Role");
            if (role != "Artist")
                return RedirectToAction("Login", "Account", new { area = "" });

            int? artistId = HttpContext.Session.GetInt32("ArtistID");
            if (artistId == null)
                return RedirectToAction("Login", "Account", new { area = "" });

            var dbSong = ctx.Songs
                            .FirstOrDefault(s => s.SongId == song.SongId &&
                                                 s.ArtistId == artistId);

            if (dbSong == null)
                return NotFound();

            dbSong.SongName = song.SongName;
            dbSong.Duration = song.Duration;
            dbSong.Category = song.Category;
            dbSong.AlbumId = song.AlbumId;

            /* ====== UPDATE FILE NHẠC ====== */
            if (uploadFile != null && uploadFile.Length > 0)
            {
                string songFolder = Path.Combine(Directory.GetCurrentDirectory(), "wwwroot/song");

                if (!string.IsNullOrEmpty(dbSong.FilePath))
                {
                    string oldPath = Path.Combine(songFolder, dbSong.FilePath);
                    if (System.IO.File.Exists(oldPath))
                        System.IO.File.Delete(oldPath);
                }

                string newName = Guid.NewGuid() + Path.GetExtension(uploadFile.FileName);
                string newPath = Path.Combine(songFolder, newName);

                using (var stream = new FileStream(newPath, FileMode.Create))
                {
                    uploadFile.CopyTo(stream);
                }

                dbSong.FilePath = newName;
            }

            /* ====== UPDATE ẢNH ====== */
            if (uploadImage != null && uploadImage.Length > 0)
            {
                string imgFolder = Path.Combine(Directory.GetCurrentDirectory(), "wwwroot/songImage");

                if (!string.IsNullOrEmpty(dbSong.SongImage))
                {
                    string oldImg = Path.Combine(imgFolder, dbSong.SongImage);
                    if (System.IO.File.Exists(oldImg))
                        System.IO.File.Delete(oldImg);
                }

                string imgName = Guid.NewGuid() + Path.GetExtension(uploadImage.FileName);
                string imgPath = Path.Combine(imgFolder, imgName);

                using (var stream = new FileStream(imgPath, FileMode.Create))
                {
                    uploadImage.CopyTo(stream);
                }

                dbSong.SongImage = imgName;
            }

            ctx.SaveChanges();
            return RedirectToAction("List");
        }
    }
}
