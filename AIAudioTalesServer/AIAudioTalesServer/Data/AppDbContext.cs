using AIAudioTalesServer.Models;
using Microsoft.EntityFrameworkCore;
using System.Diagnostics;

namespace AIAudioTalesServer.Data
{
    public class AppDbContext : DbContext
    {
        public DbSet<User> Users { get; set; }
        public DbSet<Book> Books { get; set; }
        public DbSet<PurchasedBooks> PurchasedBooks { get; set; }
        public DbSet<Story> Stories { get; set; }
        public DbSet<RefreshToken> RefreshTokens { get; set; }
        public AppDbContext(DbContextOptions options) : base(options)
        {

        }
        protected override void OnModelCreating(ModelBuilder modelBuilder)
        {
            modelBuilder.Entity<User>().HasKey(u => u.Id);
            modelBuilder.Entity<Book>().HasKey(b => b.Id);
            modelBuilder.Entity<Story>().HasKey(s => s.Id);
            modelBuilder.Entity<RefreshToken>().HasKey(t => t.UserId);

            modelBuilder.Entity<User>()
            .HasIndex(u => u.Email) 
            .IsUnique();

            modelBuilder.Entity<PurchasedBooks>().HasKey(pb => new { pb.UserId, pb.BookId });

            modelBuilder.Entity<PurchasedBooks>()
            .HasOne<User>(pb => pb.User)
            .WithMany(u => u.PurchasedBooks)
            .HasForeignKey(pb => pb.UserId)
            .OnDelete(DeleteBehavior.Restrict);


            modelBuilder.Entity<PurchasedBooks>()
            .HasOne<Book>(pb => pb.Book)
            .WithMany(b => b.PurchasedBooks)
            .HasForeignKey(pb => pb.BookId)
            .OnDelete(DeleteBehavior.Restrict);


            modelBuilder.Entity<Story>()
           .HasOne<Book>(s => s.Book)
           .WithMany(b => b.Stories)
           .HasForeignKey(s => s.BookId)
           .OnDelete(DeleteBehavior.Cascade);

            modelBuilder.Entity<User>()
            .HasOne(u => u.RefreshToken)
            .WithOne(rt => rt.User)
            .HasForeignKey<RefreshToken>(rt => rt.UserId);
        }
       
    }
}
