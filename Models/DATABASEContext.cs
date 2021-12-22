using System;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata;

#nullable disable

namespace XMUer.Models
{
    public partial class DATABASEContext : DbContext
    {
        public DATABASEContext()
        {
        }

        public DATABASEContext(DbContextOptions<DATABASEContext> options)
            : base(options)
        {
        }

        public virtual DbSet<Admin> Admins { get; set; }
        public virtual DbSet<Album> Albums { get; set; }
        public virtual DbSet<AlbumItem> AlbumItems { get; set; }
        public virtual DbSet<Avatar> Avatars { get; set; }
        public virtual DbSet<Comment> Comments { get; set; }
        public virtual DbSet<Friend> Friends { get; set; }
        public virtual DbSet<FriendApply> FriendApplies { get; set; }
        public virtual DbSet<Like> Likes { get; set; }
        public virtual DbSet<News> News { get; set; }
        public virtual DbSet<User> Users { get; set; }

        protected override void OnConfiguring(DbContextOptionsBuilder optionsBuilder)
        {
            if (!optionsBuilder.IsConfigured)
            {
#warning To protect potentially sensitive information in your connection string, you should move it out of source code. You can avoid scaffolding the connection string by using the Name= syntax to read it from configuration - see https://go.microsoft.com/fwlink/?linkid=2131148. For more guidance on storing connection strings, see http://go.microsoft.com/fwlink/?LinkId=723263.
                optionsBuilder.UseSqlServer("Data Source=122.9.40.20;Initial Catalog=DATABASE;User ID=sa;Password=P-000000;MultipleActiveResultSets=True;");
            }
        }

        protected override void OnModelCreating(ModelBuilder modelBuilder)
        {
            modelBuilder.HasAnnotation("Relational:Collation", "SQL_Latin1_General_CP1_CI_AS");

            modelBuilder.Entity<Admin>(entity =>
            {
                entity.ToTable("Admin");

                entity.Property(e => e.Id)
                    .HasMaxLength(16)
                    .IsUnicode(false);

                entity.Property(e => e.Password)
                    .IsRequired()
                    .HasMaxLength(32)
                    .IsUnicode(false);
            });

            modelBuilder.Entity<Album>(entity =>
            {
                entity.ToTable("Album");

                entity.Property(e => e.Id)
                    .HasMaxLength(50)
                    .IsUnicode(false);

                entity.Property(e => e.GmtCreate).HasColumnType("datetime");

                entity.Property(e => e.GmtModify).HasColumnType("datetime");

                entity.Property(e => e.Name).HasMaxLength(256);

                entity.Property(e => e.Path)
                    .HasMaxLength(50)
                    .IsUnicode(false);

                entity.Property(e => e.UserId)
                    .IsRequired()
                    .HasMaxLength(14)
                    .IsUnicode(false)
                    .IsFixedLength(true);

                entity.HasOne(d => d.User)
                    .WithMany(p => p.Albums)
                    .HasForeignKey(d => d.UserId)
                    .OnDelete(DeleteBehavior.ClientSetNull)
                    .HasConstraintName("FK_Album_User");
            });

            modelBuilder.Entity<AlbumItem>(entity =>
            {
                entity.ToTable("AlbumItem");

                entity.Property(e => e.Id)
                    .HasMaxLength(50)
                    .IsUnicode(false);

                entity.Property(e => e.AlbumId)
                    .IsRequired()
                    .HasMaxLength(50)
                    .IsUnicode(false);

                entity.Property(e => e.GmtCreate).HasColumnType("datetime");

                entity.Property(e => e.GmtModify).HasColumnType("datetime");

                entity.Property(e => e.Name).HasMaxLength(256);

                entity.Property(e => e.Path).HasMaxLength(256);

                entity.HasOne(d => d.Album)
                    .WithMany(p => p.AlbumItems)
                    .HasForeignKey(d => d.AlbumId)
                    .OnDelete(DeleteBehavior.ClientSetNull)
                    .HasConstraintName("FK_AlbumItem_Album");
            });

            modelBuilder.Entity<Avatar>(entity =>
            {
                entity.ToTable("Avatar");

                entity.Property(e => e.Id)
                    .HasMaxLength(50)
                    .IsUnicode(false);

                entity.Property(e => e.GmtCreate).HasColumnType("datetime");

                entity.Property(e => e.GmtModify).HasColumnType("datetime");

                entity.Property(e => e.Path).HasMaxLength(256);

                entity.Property(e => e.UserId)
                    .IsRequired()
                    .HasMaxLength(14)
                    .IsUnicode(false)
                    .IsFixedLength(true);

                entity.HasOne(d => d.User)
                    .WithMany(p => p.Avatars)
                    .HasForeignKey(d => d.UserId)
                    .OnDelete(DeleteBehavior.ClientSetNull)
                    .HasConstraintName("FK_Avatar_User");
            });

            modelBuilder.Entity<Comment>(entity =>
            {
                entity.ToTable("Comment");

                entity.Property(e => e.Id)
                    .HasMaxLength(50)
                    .IsUnicode(false);

                entity.Property(e => e.Body).HasMaxLength(1024);

                entity.Property(e => e.GmtCreate).HasColumnType("datetime");

                entity.Property(e => e.NewsId)
                    .IsRequired()
                    .HasMaxLength(50)
                    .IsUnicode(false);

                entity.Property(e => e.UserId)
                    .IsRequired()
                    .HasMaxLength(14)
                    .IsUnicode(false)
                    .IsFixedLength(true);

                entity.HasOne(d => d.News)
                    .WithMany(p => p.Comments)
                    .HasForeignKey(d => d.NewsId)
                    .OnDelete(DeleteBehavior.ClientSetNull)
                    .HasConstraintName("FK__Comment__NewsId__2BFE89A6");

                entity.HasOne(d => d.User)
                    .WithMany(p => p.Comments)
                    .HasForeignKey(d => d.UserId)
                    .OnDelete(DeleteBehavior.ClientSetNull)
                    .HasConstraintName("FK__Comment__UserId__2B0A656D");
            });

            modelBuilder.Entity<Friend>(entity =>
            {
                entity.HasKey(e => new { e.MyId, e.OtherId })
                    .HasName("PK__Friend__2AA01B961524C4A5");

                entity.ToTable("Friend");

                entity.Property(e => e.MyId)
                    .HasMaxLength(14)
                    .IsUnicode(false)
                    .IsFixedLength(true);

                entity.Property(e => e.OtherId)
                    .HasMaxLength(14)
                    .IsUnicode(false)
                    .IsFixedLength(true);

                entity.Property(e => e.GmtCreate).HasColumnType("datetime");

                entity.HasOne(d => d.My)
                    .WithMany(p => p.FriendMies)
                    .HasForeignKey(d => d.MyId)
                    .OnDelete(DeleteBehavior.ClientSetNull)
                    .HasConstraintName("FK__Friend__MyId__681373AD");

                entity.HasOne(d => d.Other)
                    .WithMany(p => p.FriendOthers)
                    .HasForeignKey(d => d.OtherId)
                    .OnDelete(DeleteBehavior.ClientSetNull)
                    .HasConstraintName("FK__Friend__OtherId__690797E6");
            });

            modelBuilder.Entity<FriendApply>(entity =>
            {
                entity.HasKey(e => new { e.FromId, e.ToId })
                    .HasName("PK__FriendAp__6232BD04727B4CCD");

                entity.ToTable("FriendApply");

                entity.Property(e => e.FromId)
                    .HasMaxLength(14)
                    .IsUnicode(false)
                    .IsFixedLength(true);

                entity.Property(e => e.ToId)
                    .HasMaxLength(14)
                    .IsUnicode(false)
                    .IsFixedLength(true);

                entity.Property(e => e.GmtCreate).HasColumnType("datetime");

                entity.HasOne(d => d.From)
                    .WithMany(p => p.FriendApplyFroms)
                    .HasForeignKey(d => d.FromId)
                    .OnDelete(DeleteBehavior.ClientSetNull)
                    .HasConstraintName("FK__FriendApp__FromI__634EBE90");

                entity.HasOne(d => d.To)
                    .WithMany(p => p.FriendApplyTos)
                    .HasForeignKey(d => d.ToId)
                    .OnDelete(DeleteBehavior.ClientSetNull)
                    .HasConstraintName("FK__FriendAppl__ToId__6442E2C9");
            });

            modelBuilder.Entity<Like>(entity =>
            {
                entity.HasKey(e => new { e.UserId, e.NewsId })
                    .HasName("PK__tmp_ms_x__3EDC2793F03943BB");

                entity.ToTable("Like");

                entity.Property(e => e.UserId)
                    .HasMaxLength(14)
                    .IsUnicode(false)
                    .IsFixedLength(true);

                entity.Property(e => e.NewsId)
                    .HasMaxLength(50)
                    .IsUnicode(false);

                entity.HasOne(d => d.News)
                    .WithMany(p => p.Likes)
                    .HasForeignKey(d => d.NewsId)
                    .OnDelete(DeleteBehavior.ClientSetNull)
                    .HasConstraintName("FK__Like__NewsId__503BEA1C");

                entity.HasOne(d => d.User)
                    .WithMany(p => p.Likes)
                    .HasForeignKey(d => d.UserId)
                    .OnDelete(DeleteBehavior.ClientSetNull)
                    .HasConstraintName("FK__Like__UserId__4F47C5E3");
            });

            modelBuilder.Entity<News>(entity =>
            {
                entity.Property(e => e.Id)
                    .HasMaxLength(50)
                    .IsUnicode(false);

                entity.Property(e => e.Body).HasMaxLength(1024);

                entity.Property(e => e.GmtCreate).HasColumnType("datetime");

                entity.Property(e => e.GmtModify).HasColumnType("datetime");

                entity.Property(e => e.IsPublic).HasDefaultValueSql("((0))");

                entity.Property(e => e.Path)
                    .HasMaxLength(50)
                    .IsUnicode(false);

                entity.Property(e => e.SharedNewsId).HasDefaultValueSql("((-1))");

                entity.Property(e => e.UserId)
                    .IsRequired()
                    .HasMaxLength(14)
                    .IsUnicode(false)
                    .IsFixedLength(true);

                entity.HasOne(d => d.User)
                    .WithMany(p => p.News)
                    .HasForeignKey(d => d.UserId)
                    .OnDelete(DeleteBehavior.ClientSetNull)
                    .HasConstraintName("FK__News__UserId__1AD3FDA4");
            });

            modelBuilder.Entity<User>(entity =>
            {
                entity.ToTable("User");

                entity.Property(e => e.Id)
                    .HasMaxLength(14)
                    .IsUnicode(false)
                    .IsFixedLength(true);

                entity.Property(e => e.Birthday).HasColumnType("date");

                entity.Property(e => e.College).HasMaxLength(128);

                entity.Property(e => e.Dept).HasMaxLength(128);

                entity.Property(e => e.Email).HasMaxLength(256);

                entity.Property(e => e.Fanime).HasMaxLength(128);

                entity.Property(e => e.Fbook).HasMaxLength(128);

                entity.Property(e => e.Fgame).HasMaxLength(128);

                entity.Property(e => e.Fhobby).HasMaxLength(128);

                entity.Property(e => e.Fmovie).HasMaxLength(128);

                entity.Property(e => e.Fmusic).HasMaxLength(128);

                entity.Property(e => e.Fsport).HasMaxLength(128);

                entity.Property(e => e.Grade).HasMaxLength(128);

                entity.Property(e => e.Hometown).HasMaxLength(256);

                entity.Property(e => e.Major).HasMaxLength(128);

                entity.Property(e => e.Name).HasMaxLength(256);

                entity.Property(e => e.Password).HasMaxLength(32);
            });

            OnModelCreatingPartial(modelBuilder);
        }

        partial void OnModelCreatingPartial(ModelBuilder modelBuilder);
    }
}
