using System;
using System.Collections.Generic;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata;

namespace LibraryCustomerSite.Models
{
    public partial class LMS_PRN221Context : DbContext
    {
        public LMS_PRN221Context()
        {
        }

        public LMS_PRN221Context(DbContextOptions<LMS_PRN221Context> options)
            : base(options)
        {
        }

        public virtual DbSet<Blog> Blogs { get; set; } = null!;
        public virtual DbSet<BookCopy> BookCopies { get; set; } = null!;
        public virtual DbSet<BookTitle> BookTitles { get; set; } = null!;
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

            if (!optionsBuilder.IsConfigured) { optionsBuilder.UseSqlServer(config.GetConnectionString("value")); }
            var config = new ConfigurationBuilder().AddJsonFile("appsettings.json").Build();

            if (!optionsBuilder.IsConfigured) { optionsBuilder.UseSqlServer(config.GetConnectionString("value")); }
        }

        protected override void OnModelCreating(ModelBuilder modelBuilder)
        {
            modelBuilder.Entity<Blog>(entity =>
            {
                entity.HasKey(e => e.Bid)
                    .HasName("PK__Blog__C6D111C94B0FC716");

                entity.ToTable("Blog");

                entity.Property(e => e.CreatedDate).HasColumnType("datetime");

                entity.Property(e => e.Title).HasMaxLength(255);

                entity.HasOne(d => d.UidNavigation)
                    .WithMany(p => p.Blogs)
                    .HasForeignKey(d => d.Uid)
                    .HasConstraintName("FK__Blog__Uid__59FA5E80");
            });

            modelBuilder.Entity<BookCopy>(entity =>
            {
                entity.ToTable("BookCopy");

                entity.Property(e => e.Id)
                    .HasMaxLength(255)
                    .IsUnicode(false);

                entity.Property(e => e.Note).HasMaxLength(255);

                entity.HasOne(d => d.BookTitle)
                    .WithMany(p => p.BookCopies)
                    .HasForeignKey(d => d.BookTitleId)
                    .HasConstraintName("FK__BookCopy__BookTi__45F365D3");
            });

            modelBuilder.Entity<BookTitle>(entity =>
            {
                entity.ToTable("BookTitle");

                entity.Property(e => e.Author).HasMaxLength(255);

                entity.Property(e => e.Bname)
                    .HasMaxLength(255)
                    .HasColumnName("BName");

                entity.Property(e => e.Cid).HasColumnName("CId");

                entity.Property(e => e.Description).HasMaxLength(255);

                entity.Property(e => e.Image).HasMaxLength(255);

                entity.Property(e => e.Price).HasColumnType("decimal(10, 2)");

                entity.Property(e => e.PublishDate).HasColumnType("datetime");

                entity.HasOne(d => d.CidNavigation)
                    .WithMany(p => p.BookTitles)
                    .HasForeignKey(d => d.Cid)
                    .HasConstraintName("FK__BookTitle__CId__412EB0B6");

                entity.HasOne(d => d.Publisher)
                    .WithMany(p => p.BookTitles)
                    .HasForeignKey(d => d.PublisherId)
                    .HasConstraintName("FK__BookTitle__Publi__4316F928");
            });

            modelBuilder.Entity<BorrowInformation>(entity =>
            {
                entity.HasKey(e => e.Oid)
                    .HasName("PK__BorrowIn__CB3E4F31CC88F981");

                entity.ToTable("BorrowInformation");

                entity.Property(e => e.BorrowDate).HasColumnType("datetime");

                entity.Property(e => e.CheckoutDate).HasColumnType("datetime");

                entity.Property(e => e.DueDate).HasColumnType("datetime");

                entity.Property(e => e.ReturnDate).HasColumnType("datetime");

                entity.Property(e => e.TotalAmount).HasColumnType("decimal(10, 2)");

                entity.HasOne(d => d.UidNavigation)
                    .WithMany(p => p.BorrowInformations)
                    .HasForeignKey(d => d.Uid)
                    .HasConstraintName("FK__BorrowInfor__Uid__534D60F1");

                entity.HasMany(d => d.Bids)
                    .WithMany(p => p.Oids)
                    .UsingEntity<Dictionary<string, object>>(
                        "BorrowDetail",
                        l => l.HasOne<BookCopy>().WithMany().HasForeignKey("Bid").OnDelete(DeleteBehavior.ClientSetNull).HasConstraintName("FK__BorrowDetai__Bid__571DF1D5"),
                        r => r.HasOne<BorrowInformation>().WithMany().HasForeignKey("Oid").OnDelete(DeleteBehavior.ClientSetNull).HasConstraintName("FK__BorrowDetai__Oid__5629CD9C"),
                        j =>
                        {
                            j.HasKey("Oid", "Bid").HasName("PK__BorrowDe__47535E2D6C560374");

                            j.ToTable("BorrowDetail");

                            j.IndexerProperty<string>("Bid").HasMaxLength(255).IsUnicode(false);
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
                    .HasConstraintName("FK__Feedback__Bid__48CFD27E");

                entity.HasOne(d => d.UidNavigation)
                    .WithMany(p => p.Feedbacks)
                    .HasForeignKey(d => d.Uid)
                    .HasConstraintName("FK__Feedback__Uid__49C3F6B7");
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
                    .HasName("PK__User__C5B69A4AF284BAF6");

                entity.ToTable("User");

                entity.HasIndex(e => e.Email, "UQ__User__A9D105349423F8FA")
                    .IsUnique();

                entity.Property(e => e.Email).HasMaxLength(255);

                entity.Property(e => e.FullName).HasMaxLength(255);

                entity.Property(e => e.Image)
                    .HasMaxLength(255)
                    .IsUnicode(false);

                entity.Property(e => e.Password)
                    .HasMaxLength(255)
                    .IsUnicode(false);

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
                    .HasConstraintName("FK__Wishlist__Uid__4CA06362");

                entity.HasMany(d => d.Bids)
                    .WithMany(p => p.Wids)
                    .UsingEntity<Dictionary<string, object>>(
                        "WishlistItem",
                        l => l.HasOne<BookCopy>().WithMany().HasForeignKey("Bid").OnDelete(DeleteBehavior.ClientSetNull).HasConstraintName("FK__WishlistIte__BId__5070F446"),
                        r => r.HasOne<Wishlist>().WithMany().HasForeignKey("Wid").OnDelete(DeleteBehavior.ClientSetNull).HasConstraintName("FK__WishlistIte__WId__4F7CD00D"),
                        j =>
                        {
                            j.HasKey("Wid", "Bid").HasName("PK__Wishlist__D75A85F56E13CFE3");

                            j.ToTable("WishlistItem");

                            j.IndexerProperty<int>("Wid").HasColumnName("WId");

                            j.IndexerProperty<string>("Bid").HasMaxLength(255).IsUnicode(false).HasColumnName("BId");
                        });
            });

            OnModelCreatingPartial(modelBuilder);
        }

        partial void OnModelCreatingPartial(ModelBuilder modelBuilder);
    }
}
