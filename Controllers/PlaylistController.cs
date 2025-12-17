using Microsoft.AspNetCore.Mvc;

namespace MusicDBApp.Controllers
{
    public class PlaylistController : Controller
    {
        public IActionResult ListPlaylist()
        {
            return View();
        }
    }
}
