using Microsoft.AspNetCore.Mvc;
using MusicDBApp.Models;

namespace MusicDBApp.Controllers
{
    public class FavouriteSongController : Controller
    {
        public MusicDbContext ctx;
        public FavouriteSongController(MusicDbContext ctx)
        {
            this.ctx = ctx;
        }
        public IActionResult ListSong()
        { 
            var songs = ctx.Songs.ToList();
            return View(songs);
        }
    }
}
