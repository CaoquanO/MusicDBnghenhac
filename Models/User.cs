using System;
using System.Collections.Generic;

namespace MusicDBApp.Models;

public partial class User
{
    public int UserId { get; set; }

    public string UserName { get; set; } = null!;

    public string Email { get; set; } = null!;

    public string? FullName { get; set; }

    public string Password { get; set; } = null!;

    public DateOnly? DateOfBirth { get; set; }

    public string RoleId { get; set; } = null!;

    public virtual ICollection<Admin> Admins { get; set; } = new List<Admin>();

    public virtual ICollection<Artist> Artists { get; set; } = new List<Artist>();

    public virtual ICollection<Playlist> Playlists { get; set; } = new List<Playlist>();

    public virtual Role Role { get; set; } = null!;

    public virtual ICollection<Song> Songs { get; set; } = new List<Song>();
}
