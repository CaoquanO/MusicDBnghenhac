using System;
using System.Collections.Generic;

namespace MusicDBApp.Models;

public partial class Song
{
    public int SongId { get; set; }

    public string SongName { get; set; } = null!;

    public DateTime UploadDate { get; set; }

    public string SongImage { get; set; } = null!;

    public TimeOnly?  Duration { get; set; }

    public string? Category { get; set; }

    public string? FilePath { get; set; }

    public int ArtistId { get; set; }

    public int? AlbumId { get; set; }

    public int ListenCount { get; set; }

    public virtual Album? Album { get; set; }

    public virtual Artist Artist { get; set; } = null!;

    public virtual ICollection<Playlist> Playlists { get; set; } = new List<Playlist>();

    public virtual ICollection<User> Users { get; set; } = new List<User>();
}
