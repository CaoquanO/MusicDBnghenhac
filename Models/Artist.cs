using System;
using System.Collections.Generic;

namespace MusicDBApp.Models;

public partial class Artist
{
    public int ArtistId { get; set; }

    public string ArtistName { get; set; } = null!;

    public string? Bio { get; set; }

    public string? Country { get; set; }

    public DateOnly? DateOfBirth { get; set; }

    public int UserId { get; set; }

    public virtual ICollection<Album> Albums { get; set; } = new List<Album>();

    public virtual ICollection<Song> Songs { get; set; } = new List<Song>();

    public virtual User User { get; set; } = null!;
}
