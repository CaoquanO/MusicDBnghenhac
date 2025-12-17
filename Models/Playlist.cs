using System;
using System.Collections.Generic;

namespace MusicDBApp.Models;

public partial class Playlist
{
    public int PlaylistId { get; set; }

    public string PlaylistName { get; set; } = null!;

    public int UserId { get; set; }

    public DateTime? CreateDate { get; set; }

    public string? IsPublic { get; set; }

    public virtual User User { get; set; } = null!;

    public virtual ICollection<Song> Songs { get; set; } = new List<Song>();
}
