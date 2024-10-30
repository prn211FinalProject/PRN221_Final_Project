using System;
using System.Collections.Generic;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata;
using Microsoft.Extensions.Configuration;

namespace LibraryAdminSite.Models
{
    public partial class LMS_PRN221Context : DbContext
    {
        public static LMS_PRN221Context Ins = new LMS_PRN221Context();  
        public LMS_PRN221Context()
        {
            if (Ins == null) Ins = this;

        }

        public LMS_PRN221Context(DbContextOptions<LMS_PRN221Context> options)
            : base(options)
        {
        }

        public virtual DbSet<Blog> Blogs { get; set; } = null!;
        public virtual DbSet<Book> Books { get; set; } = null!;
        public virtual DbSet<BorrowInformation> BorrowInformations { get; set; } = null!;
        public virtual DbSet<Category> Categories { get; set; } = null!;
        public virtual DbSet<Feedback> Feedbacks { get; set; } = null!;
        public virtual DbSet<Publisher> Publishers { get; set; } = null!;
        public virtual DbSet<Role> Roles { get; set; } = null!;
        public virtual DbSet<User> Users { get; set; } = null!;
        public virtual DbSet<Wishlist> Wishlists { get; set; } = null!;

        protected override void OnConfiguring(DbContextOptionsBuilder optionsBuilder)
        {
            var config = new ConfigurationBuilder().AddJsonFile("appsettings.json").Build();
            if (!optionsBuilder.IsConfigured)
            {
                optionsBuilder.UseSqlServer(config.GetConnectionString("value"));
            }
        }

        protected override void OnModelCreating(ModelBuilder modelBuilder)
        {
            modelBuilder.Entity<Blog>(entity =>
            {
                entity.HasKey(e => e.Bid)
                    .HasName("PK__Blog__C6D111C9403064F6");

                entity.ToTable("Blog");

                entity.Property(e => e.CreatedDate).HasColumnType("datetime");

                entity.Property(e => e.Title).HasMaxLength(255);

                entity.HasOne(d => d.UidNavigation)
                    .WithMany(p => p.Blogs)
                    .HasForeignKey(d => d.Uid)
                    .HasConstraintName("FK__Blog__Uid__5629CD9C");
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

                entity.Property(e => e.PublishDate).HasColumnType("datetime");

                entity.HasOne(d => d.CidNavigation)
                    .WithMany(p => p.Books)
                    .HasForeignKey(d => d.Cid)
                    .HasConstraintName("FK__Book__CId__412EB0B6");

                entity.HasOne(d => d.Publisher)
                    .WithMany(p => p.Books)
                    .HasForeignKey(d => d.PublisherId)
                    .HasConstraintName("FK__Book__PublisherI__4222D4EF");
            });

            modelBuilder.Entity<BorrowInformation>(entity =>
            {
                entity.HasKey(e => e.Oid)
                    .HasName("PK__BorrowIn__CB3E4F316513637D");

                entity.ToTable("BorrowInformation");

                entity.Property(e => e.BorrowDate).HasColumnType("datetime");

                entity.Property(e => e.DueDate).HasColumnType("datetime");

                entity.Property(e => e.TotalAmount).HasColumnType("decimal(10, 2)");

                entity.HasOne(d => d.UidNavigation)
                    .WithMany(p => p.BorrowInformations)
                    .HasForeignKey(d => d.Uid)
                    .HasConstraintName("FK__BorrowInfor__Uid__4F7CD00D");

                entity.HasMany(d => d.Bids)
                    .WithMany(p => p.Oids)
                    .UsingEntity<Dictionary<string, object>>(
                        "BorrowDetail",
                        l => l.HasOne<Book>().WithMany().HasForeignKey("Bid").OnDelete(DeleteBehavior.ClientSetNull).HasConstraintName("FK__BorrowDetai__Bid__534D60F1"),
                        r => r.HasOne<BorrowInformation>().WithMany().HasForeignKey("Oid").OnDelete(DeleteBehavior.ClientSetNull).HasConstraintName("FK__BorrowDetai__Oid__52593CB8"),
                        j =>
                        {
                            j.HasKey("Oid", "Bid").HasName("PK__BorrowDe__47535E2DAE83CCA7");

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
                    .HasConstraintName("FK__Feedback__Bid__44FF419A");

                entity.HasOne(d => d.UidNavigation)
                    .WithMany(p => p.Feedbacks)
                    .HasForeignKey(d => d.Uid)
                    .HasConstraintName("FK__Feedback__Uid__45F365D3");
            });

            modelBuilder.Entity<Publisher>(entity =>
            {
                entity.ToTable("Publisher");

                entity.Property(e => e.Address).HasMaxLength(255);

                entity.Property(e => e.Email).HasMaxLength(255);

                entity.Property(e => e.Phone).HasMaxLength(20);

                entity.Property(e => e.Pname)
                    .HasMaxLength(255)
                    .HasColumnName("PName");
            });

            modelBuilder.Entity<Role>(entity =>
            {
                entity.ToTable("Role");

                entity.Property(e => e.Name).HasMaxLength(255);
            });

            modelBuilder.Entity<User>(entity =>
            {
                entity.HasKey(e => e.Uid)
                    .HasName("PK__User__C5B69A4A5DC05AB8");

                entity.ToTable("User");

                entity.HasIndex(e => e.Email, "UQ__User__A9D10534C5E19239")
                    .IsUnique();

                entity.Property(e => e.Email).HasMaxLength(255);

                entity.Property(e => e.FullName).HasMaxLength(255);

                entity.Property(e => e.Phone).HasMaxLength(20);

                entity.HasOne(d => d.Role)
                    .WithMany(p => p.Users)
                    .HasForeignKey(d => d.RoleId)
                    .HasConstraintName("FK__User__RoleId__3A81B327");
            });

            modelBuilder.Entity<Wishlist>(entity =>
            {
                entity.ToTable("Wishlist");

                entity.HasOne(d => d.UidNavigation)
                    .WithMany(p => p.Wishlists)
                    .HasForeignKey(d => d.Uid)
                    .HasConstraintName("FK__Wishlist__Uid__48CFD27E");

                entity.HasMany(d => d.Bids)
                    .WithMany(p => p.Wids)
                    .UsingEntity<Dictionary<string, object>>(
                        "WishlistItem",
                        l => l.HasOne<Book>().WithMany().HasForeignKey("Bid").OnDelete(DeleteBehavior.ClientSetNull).HasConstraintName("FK__WishlistIte__BId__4CA06362"),
                        r => r.HasOne<Wishlist>().WithMany().HasForeignKey("Wid").OnDelete(DeleteBehavior.ClientSetNull).HasConstraintName("FK__WishlistIte__WId__4BAC3F29"),
                        j =>
                        {
                            j.HasKey("Wid", "Bid").HasName("PK__Wishlist__D75A85F508B32E90");

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
