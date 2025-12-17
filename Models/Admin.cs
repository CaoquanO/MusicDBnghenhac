using System;
using System.Collections.Generic;

namespace MusicDBApp.Models;

public partial class Admin
{
    public int AdminId { get; set; }

    public string AdminName { get; set; } = null!;

    public int UserId { get; set; }

    public virtual User User { get; set; } = null!;
}
