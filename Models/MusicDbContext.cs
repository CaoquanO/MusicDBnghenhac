using System;
using System.Collections.Generic;
using Microsoft.EntityFrameworkCore;

namespace MusicDBApp.Models;

public partial class MusicDbContext : DbContext
{
    public MusicDbContext()
    {
    }

    public MusicDbContext(DbContextOptions<MusicDbContext> options)
        : base(options)
    {
    }

    public virtual DbSet<Admin> Admins { get; set; }

    public virtual DbSet<Album> Albums { get; set; }

    public virtual DbSet<Artist> Artists { get; set; }

    public virtual DbSet<Playlist> Playlists { get; set; }

    public virtual DbSet<Role> Roles { get; set; }

    public virtual DbSet<Song> Songs { get; set; }

    public virtual DbSet<User> Users { get; set; }

    protected override void OnConfiguring(DbContextOptionsBuilder optionsBuilder)
#warning To protect potentially sensitive information in your connection string, you should move it out of source code. You can avoid scaffolding the connection string by using the Name= syntax to read it from configuration - see https://go.microsoft.com/fwlink/?linkid=2131148. For more guidance on storing connection strings, see https://go.microsoft.com/fwlink/?LinkId=723263.
        => optionsBuilder.UseSqlServer("Data Source=.;Initial Catalog=MusicDB;Integrated Security=True;Connect Timeout=30;Encrypt=True;Trust Server Certificate=True;Application Intent=ReadWrite;Multi Subnet Failover=False");

    protected override void OnModelCreating(ModelBuilder modelBuilder)
    {
        modelBuilder.Entity<Admin>(entity =>
        {
            entity.HasKey(e => e.AdminId).HasName("PK__Admin__719FE4E8852ED324");

            entity.ToTable("Admin");

            entity.Property(e => e.AdminId)
                .ValueGeneratedNever()
                .HasColumnName("AdminID");
            entity.Property(e => e.AdminName).HasMaxLength(200);
            entity.Property(e => e.UserId).HasColumnName("UserID");

            entity.HasOne(d => d.User).WithMany(p => p.Admins)
                .HasForeignKey(d => d.UserId)
                .OnDelete(DeleteBehavior.ClientSetNull)
                .HasConstraintName("FK_Admin_UserID");
        });

        modelBuilder.Entity<Album>(entity =>
        {
            entity.HasKey(e => e.AlbumId).HasName("PK__Album__97B4BE175D23C710");

            entity.ToTable("Album");

            entity.Property(e => e.AlbumId).HasColumnName("AlbumID");
            entity.Property(e => e.AlbumName).HasMaxLength(200);
            entity.Property(e => e.ArtistId).HasColumnName("ArtistID");
            entity.Property(e => e.CoverImage).HasMaxLength(1000);
            entity.Property(e => e.Title).HasMaxLength(1000);

            entity.HasOne(d => d.Artist).WithMany(p => p.Albums)
                .HasForeignKey(d => d.ArtistId)
                .OnDelete(DeleteBehavior.ClientSetNull)
                .HasConstraintName("FK_Album_ArtistID");
        });

        modelBuilder.Entity<Artist>(entity =>
        {
            entity.HasKey(e => e.ArtistId).HasName("PK__Artist__25706B70DFEF83BE");

            entity.ToTable("Artist");

            entity.Property(e => e.ArtistId).HasColumnName("ArtistID");
            entity.Property(e => e.ArtistName).HasMaxLength(200);
            entity.Property(e => e.Country).HasMaxLength(100);
            entity.Property(e => e.UserId).HasColumnName("UserID");

            entity.HasOne(d => d.User).WithMany(p => p.Artists)
                .HasForeignKey(d => d.UserId)
                .OnDelete(DeleteBehavior.ClientSetNull)
                .HasConstraintName("FK_Artist_UserID");
        });

        modelBuilder.Entity<Playlist>(entity =>
        {
            entity.HasKey(e => e.PlaylistId).HasName("PK__Playlist__B30167808EE994FC");

            entity.ToTable("Playlist");

            entity.Property(e => e.PlaylistId).HasColumnName("PlaylistID");
            entity.Property(e => e.CreateDate)
                .HasDefaultValueSql("(getdate())")
                .HasColumnType("datetime");
            entity.Property(e => e.IsPublic).HasMaxLength(100);
            entity.Property(e => e.PlaylistName).HasMaxLength(100);
            entity.Property(e => e.UserId).HasColumnName("UserID");

            entity.HasOne(d => d.User).WithMany(p => p.Playlists)
                .HasForeignKey(d => d.UserId)
                .OnDelete(DeleteBehavior.ClientSetNull)
                .HasConstraintName("FK_Playlist_UserID");

            entity.HasMany(d => d.Songs).WithMany(p => p.Playlists)
                .UsingEntity<Dictionary<string, object>>(
                    "PlaylistSong",
                    r => r.HasOne<Song>().WithMany()
                        .HasForeignKey("SongId")
                        .OnDelete(DeleteBehavior.ClientSetNull)
                        .HasConstraintName("FK_PlaylistSong_SongID"),
                    l => l.HasOne<Playlist>().WithMany()
                        .HasForeignKey("PlaylistId")
                        .OnDelete(DeleteBehavior.ClientSetNull)
                        .HasConstraintName("FK_PlaylistSong_PlaylistID"),
                    j =>
                    {
                        j.HasKey("PlaylistId", "SongId").HasName("PK__Playlist__D22F5AEF8813D086");
                        j.ToTable("Playlist_Song");
                        j.IndexerProperty<int>("PlaylistId").HasColumnName("PlaylistID");
                        j.IndexerProperty<int>("SongId").HasColumnName("SongID");
                    });
        });

        modelBuilder.Entity<Role>(entity =>
        {
            entity.HasKey(e => e.RoleId).HasName("PK__Role__8AFACE3ACFD38AC3");

            entity.ToTable("Role");

            entity.Property(e => e.RoleId)
                .HasMaxLength(50)
                .IsUnicode(false)
                .HasColumnName("RoleID");
            entity.Property(e => e.RoleName).HasMaxLength(50);
        });

        modelBuilder.Entity<Song>(entity =>
        {
            entity.HasKey(e => e.SongId).HasName("PK__Song__12E3D6F78D4EE7CD");

            entity.ToTable("Song");

            entity.Property(e => e.SongId).HasColumnName("SongID");
            entity.Property(e => e.AlbumId).HasColumnName("AlbumID");
            entity.Property(e => e.ArtistId).HasColumnName("ArtistID");
            entity.Property(e => e.Category).HasMaxLength(100);
            entity.Property(e => e.FilePath).HasMaxLength(1000);
            entity.Property(e => e.SongImage)
                .HasMaxLength(200)
                .IsUnicode(false);
            entity.Property(e => e.SongName).HasMaxLength(200);
            entity.Property(e => e.UploadDate)
                .HasDefaultValueSql("(getdate())")
                .HasColumnType("datetime");

            entity.HasOne(d => d.Album).WithMany(p => p.Songs)
                .HasForeignKey(d => d.AlbumId)
                .HasConstraintName("FK_Song_AlbumID");

            entity.HasOne(d => d.Artist).WithMany(p => p.Songs)
                .HasForeignKey(d => d.ArtistId)
                .OnDelete(DeleteBehavior.ClientSetNull)
                .HasConstraintName("FK_Song_ArtistID");
        });

        modelBuilder.Entity<User>(entity =>
        {
            entity.HasKey(e => e.UserId).HasName("PK__User__1788CCAC46E86DB8");

            entity.ToTable("User");

            entity.HasIndex(e => e.Email, "UQ_User_Email").IsUnique();

            entity.Property(e => e.UserId).HasColumnName("UserID");
            entity.Property(e => e.Email).HasMaxLength(100);
            entity.Property(e => e.FullName).HasMaxLength(200);
            entity.Property(e => e.Password).HasMaxLength(255);
            entity.Property(e => e.RoleId)
                .HasMaxLength(50)
                .IsUnicode(false)
                .HasColumnName("RoleID");
            entity.Property(e => e.UserName).HasMaxLength(100);

            entity.HasOne(d => d.Role).WithMany(p => p.Users)
                .HasForeignKey(d => d.RoleId)
                .OnDelete(DeleteBehavior.ClientSetNull)
                .HasConstraintName("FK_User_RoleID");

            entity.HasMany(d => d.Songs).WithMany(p => p.Users)
                .UsingEntity<Dictionary<string, object>>(
                    "FavoriteSong",
                    r => r.HasOne<Song>().WithMany()
                        .HasForeignKey("SongId")
                        .OnDelete(DeleteBehavior.ClientSetNull)
                        .HasConstraintName("FK_Favorite_Song_SongID"),
                    l => l.HasOne<User>().WithMany()
                        .HasForeignKey("UserId")
                        .OnDelete(DeleteBehavior.ClientSetNull)
                        .HasConstraintName("FK_Favorite_UserID"),
                    j =>
                    {
                        j.HasKey("UserId", "SongId").HasName("PK__Favorite__76A6F1C3CB898530");
                        j.ToTable("Favorite_Song");
                        j.IndexerProperty<int>("UserId").HasColumnName("UserID");
                        j.IndexerProperty<int>("SongId").HasColumnName("SongID");
                    });
        });

        OnModelCreatingPartial(modelBuilder);
    }

    partial void OnModelCreatingPartial(ModelBuilder modelBuilder);
}
