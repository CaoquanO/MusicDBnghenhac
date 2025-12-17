using Microsoft.AspNetCore.Hosting;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using MusicDBApp.Models;

namespace MusicDBApp.Areas.Admin.Controllers
{
    [Area("Admin")]
    public class SongController : Controller
    {
        public MusicDbContext ctx;
        private readonly IWebHostEnvironment env;

        public SongController(MusicDbContext ctx, IWebHostEnvironment env)
        {
            this.ctx = ctx;
            this.env = env;
        }
        public IActionResult List()
        {
            List<Song> songs = ctx.Songs.Include(u => u.Artist).Include(u => u.Album).ToList();
            return View(songs);
        }
        [HttpGet]
        public IActionResult AddNew() {
            ViewBag.Artists = ctx.Artists.ToList();
            return View();
        }
        [HttpPost]
        public IActionResult AddNew(Song song, IFormFile uploadFile, IFormFile uploadImage)
        {
            // Upload file nhạc
            if (uploadFile != null && uploadFile.Length > 0)
            {
                string songFolder = Path.Combine(env.WebRootPath, "song");
                Directory.CreateDirectory(songFolder);

                string fileName = Guid.NewGuid() + Path.GetExtension(uploadFile.FileName);
                string path = Path.Combine(songFolder, fileName);

                using (var stream = new FileStream(path, FileMode.Create))
                {
                    uploadFile.CopyTo(stream);
                }

                song.FilePath = fileName;
            }

            // Upload ảnh
            if (uploadImage != null && uploadImage.Length > 0)
            {
                string imgFolder = Path.Combine(env.WebRootPath, "songImage");
                Directory.CreateDirectory(imgFolder);

                string imgName = Guid.NewGuid() + Path.GetExtension(uploadImage.FileName);
                string imgPath = Path.Combine(imgFolder, imgName);

                using (var stream = new FileStream(imgPath, FileMode.Create))
                {
                    uploadImage.CopyTo(stream);
                }

                song.SongImage = imgName;
            }

            ctx.Songs.Add(song);   // SongId tự sinh
            ctx.SaveChanges();

            return RedirectToAction("List");
        }
        [HttpPost]
        public IActionResult Find()
        {
            string name = Request.Form["searchName"].ToString();
           List<Song> songs=ctx.Songs.Where(u => u.SongName!.Contains(name))
                .Include(u => u.Artist)
                .Include(u => u.Album)
                .ToList();
            return View(songs);
        }
        public IActionResult Delete(int id)
        {
            Song c = ctx.Songs.Where(u => u.SongId == id).FirstOrDefault();
            ctx.Songs.Remove(c);
            ctx.SaveChanges();
            return RedirectToAction("List");
        }
        [HttpGet]
        public IActionResult Update(int id)
        {
            Song song = ctx.Songs
                           .Include(s => s.Artist)
                           .FirstOrDefault(s => s.SongId == id);
            ViewBag.Artists = ctx.Artists.ToList();
            return View(song);
        }
        [HttpPost]
        public IActionResult Update(Song song, IFormFile uploadFile, IFormFile uploadImage)
        {
            Song dbSong = ctx.Songs.FirstOrDefault(s => s.SongId == song.SongId);
            if (dbSong == null)
                return NotFound();

            // Update thông tin text
            dbSong.SongName = song.SongName;
            dbSong.Duration = song.Duration;
            dbSong.Category = song.Category;
            dbSong.ArtistId = song.ArtistId;

            /* ================== UPDATE FILE NHẠC ================== */
            if (uploadFile != null && uploadFile.Length > 0)
            {
                string songFolder = Path.Combine(env.WebRootPath, "song");
                Directory.CreateDirectory(songFolder);

                // xóa file cũ
                if (!string.IsNullOrEmpty(dbSong.FilePath))
                {
                    string oldSongPath = Path.Combine(songFolder, dbSong.FilePath);
                    if (System.IO.File.Exists(oldSongPath))
                        System.IO.File.Delete(oldSongPath);
                }

                string newSongName = Guid.NewGuid() + Path.GetExtension(uploadFile.FileName);
                string newSongPath = Path.Combine(songFolder, newSongName);

                using (var stream = new FileStream(newSongPath, FileMode.Create))
                {
                    uploadFile.CopyTo(stream);
                }

                dbSong.FilePath = newSongName;
            }

            /* ================== UPDATE ẢNH ================== */
            if (uploadImage != null && uploadImage.Length > 0)
            {
                string imgFolder = Path.Combine(env.WebRootPath, "songImage");
                Directory.CreateDirectory(imgFolder);

                // xóa ảnh cũ
                if (!string.IsNullOrEmpty(dbSong.SongImage))
                {
                    string oldImgPath = Path.Combine(imgFolder, dbSong.SongImage);
                    if (System.IO.File.Exists(oldImgPath))
                        System.IO.File.Delete(oldImgPath);
                }

                string newImgName = Guid.NewGuid() + Path.GetExtension(uploadImage.FileName);
                string newImgPath = Path.Combine(imgFolder, newImgName);

                using (var stream = new FileStream(newImgPath, FileMode.Create))
                {
                    uploadImage.CopyTo(stream);
                }

                dbSong.SongImage = newImgName;
            }

            ctx.SaveChanges();
            return RedirectToAction("List");
        }


    }
}
