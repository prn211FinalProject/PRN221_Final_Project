using System;
using System.Collections.Generic;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata;

namespace LibraryCustomerSite.Models
{
    public partial class LibraryManagementContext : DbContext
    {
        public static LibraryManagementContext Ins = new LibraryManagementContext();

        public LibraryManagementContext()
        {
            if (Ins == null)
            {
                Ins = this;
            }
        }

        public LibraryManagementContext(DbContextOptions<LibraryManagementContext> options)
            : base(options)
        {
        }

        public virtual DbSet<Blog> Blogs { get; set; } = null!;
        public virtual DbSet<Book> Books { get; set; } = null!;
        public virtual DbSet<BorrowInformation> BorrowInformations { get; set; } = null!;
        public virtual DbSet<Category> Categories { get; set; } = null!;
        public virtual DbSet<Feedback> Feedbacks { get; set; } = null!;
        public virtual DbSet<Role> Roles { get; set; } = null!;
        public virtual DbSet<User> Users { get; set; } = null!;
        public virtual DbSet<Wishlist> Wishlists { get; set; } = null!;

        protected override void OnConfiguring(DbContextOptionsBuilder optionsBuilder)
        {
            //            if (!optionsBuilder.IsConfigured)
            //            {
            //#warning To protect potentially sensitive information in your connection string, you should move it out of source code. You can avoid scaffolding the connection string by using the Name= syntax to read it from configuration - see https://go.microsoft.com/fwlink/?linkid=2131148. For more guidance on storing connection strings, see http://go.microsoft.com/fwlink/?LinkId=723263.
            //                optionsBuilder.UseSqlServer("Data Source=HUNGNGO\\HUNGNGO;Initial Catalog=PRN211_1; Trusted_Connection=SSPI;Encrypt=false;TrustServerCertificate=true");
            //            }
            var config = new ConfigurationBuilder().AddJsonFile("appsettings.json").Build();

            if (!optionsBuilder.IsConfigured) { optionsBuilder.UseSqlServer(config.GetConnectionString("value")); }
        }

        protected override void OnModelCreating(ModelBuilder modelBuilder)
        {
            modelBuilder.Entity<Blog>(entity =>
            {
                entity.HasKey(e => e.Bid)
                    .HasName("PK__Blog__C6D111C9BB6BF98F");

                entity.ToTable("Blog");

                entity.Property(e => e.CreatedDate).HasColumnType("datetime");

                entity.Property(e => e.Title).HasMaxLength(255);

                entity.HasOne(d => d.UidNavigation)
                    .WithMany(p => p.Blogs)
                    .HasForeignKey(d => d.Uid)
                    .HasConstraintName("FK__Blog__Uid__403A8C7D");
            });

            modelBuilder.Entity<Book>(entity =>
            {
                entity.ToTable("Book");

                entity.Property(e => e.Author).HasMaxLength(255);

                entity.Property(e => e.Bname)
                    .HasMaxLength(255)
                    .HasColumnName("BName");

                entity.Property(e => e.Cid).HasColumnName("CId");

                entity.Property(e => e.Image).HasMaxLength(255);

                entity.Property(e => e.Price).HasColumnType("decimal(10, 2)");

                entity.HasOne(d => d.CidNavigation)
                    .WithMany(p => p.Books)
                    .HasForeignKey(d => d.Cid)
                    .HasConstraintName("FK__Book__CId__2C3393D0");
            });

            modelBuilder.Entity<BorrowInformation>(entity =>
            {
                entity.HasKey(e => e.Oid)
                    .HasName("PK__BorrowIn__CB3E4F31B4D0125B");

                entity.ToTable("BorrowInformation");

                entity.Property(e => e.BorrowDate).HasColumnType("datetime");

                entity.Property(e => e.DueDate).HasColumnType("datetime");

                entity.Property(e => e.TotalAmount).HasColumnType("decimal(10, 2)");

                entity.HasOne(d => d.UidNavigation)
                    .WithMany(p => p.BorrowInformations)
                    .HasForeignKey(d => d.Uid)
                    .HasConstraintName("FK__BorrowInfor__Uid__398D8EEE");

                entity.HasMany(d => d.Bids)
                    .WithMany(p => p.Oids)
                    .UsingEntity<Dictionary<string, object>>(
                        "BorrowDetail",
                        l => l.HasOne<Book>().WithMany().HasForeignKey("Bid").OnDelete(DeleteBehavior.ClientSetNull).HasConstraintName("FK__BorrowDetai__Bid__3D5E1FD2"),
                        r => r.HasOne<BorrowInformation>().WithMany().HasForeignKey("Oid").OnDelete(DeleteBehavior.ClientSetNull).HasConstraintName("FK__BorrowDetai__Oid__3C69FB99"),
                        j =>
                        {
                            j.HasKey("Oid", "Bid").HasName("PK__BorrowDe__47535E2D3474CB2B");

                            j.ToTable("BorrowDetail");
                        });
            });

            modelBuilder.Entity<Category>(entity =>
            {
                entity.ToTable("Category");

                entity.Property(e => e.Cname)
                    .HasMaxLength(255)
                    .HasColumnName("CName");
            });

            modelBuilder.Entity<Feedback>(entity =>
            {
                entity.ToTable("Feedback");

                entity.Property(e => e.CreatedDate).HasColumnType("datetime");

                entity.HasOne(d => d.BidNavigation)
                    .WithMany(p => p.Feedbacks)
                    .HasForeignKey(d => d.Bid)
                    .HasConstraintName("FK__Feedback__Bid__2F10007B");

                entity.HasOne(d => d.UidNavigation)
                    .WithMany(p => p.Feedbacks)
                    .HasForeignKey(d => d.Uid)
                    .HasConstraintName("FK__Feedback__Uid__300424B4");
            });

            modelBuilder.Entity<Role>(entity =>
            {
                entity.ToTable("Role");

                entity.Property(e => e.Name).HasMaxLength(255);
            });

            modelBuilder.Entity<User>(entity =>
            {
                entity.HasKey(e => e.Uid)
                    .HasName("PK__User__C5B69A4A190D22E9");

                entity.ToTable("User");

                entity.HasIndex(e => e.Email, "UQ__User__A9D1053420587E18")
                    .IsUnique();

                entity.Property(e => e.Email).HasMaxLength(255);

                entity.Property(e => e.FullName).HasMaxLength(255);

                entity.Property(e => e.Phone).HasMaxLength(20);

                entity.HasOne(d => d.Role)
                    .WithMany(p => p.Users)
                    .HasForeignKey(d => d.RoleId)
                    .HasConstraintName("FK__User__RoleId__276EDEB3");
            });

            modelBuilder.Entity<Wishlist>(entity =>
            {
                entity.ToTable("Wishlist");

                entity.HasOne(d => d.UidNavigation)
                    .WithMany(p => p.Wishlists)
                    .HasForeignKey(d => d.Uid)
                    .HasConstraintName("FK__Wishlist__Uid__32E0915F");

                entity.HasMany(d => d.Bids)
                    .WithMany(p => p.Wids)
                    .UsingEntity<Dictionary<string, object>>(
                        "WishlistItem",
                        l => l.HasOne<Book>().WithMany().HasForeignKey("Bid").OnDelete(DeleteBehavior.ClientSetNull).HasConstraintName("FK__WishlistIte__BId__36B12243"),
                        r => r.HasOne<Wishlist>().WithMany().HasForeignKey("Wid").OnDelete(DeleteBehavior.ClientSetNull).HasConstraintName("FK__WishlistIte__WId__35BCFE0A"),
                        j =>
                        {
                            j.HasKey("Wid", "Bid").HasName("PK__Wishlist__D75A85F58AE6D559");

                            j.ToTable("WishlistItem");

                            j.IndexerProperty<int>("Wid").HasColumnName("WId");

                            j.IndexerProperty<int>("Bid").HasColumnName("BId");
                        });
            });

            OnModelCreatingPartial(modelBuilder);
        }

        partial void OnModelCreatingPartial(ModelBuilder modelBuilder);
    }
}
