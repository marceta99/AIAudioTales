using AIAudioTalesServer.Data.Seed.BooksPictures;
using AIAudioTalesServer.Models;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Infrastructure;
using Microsoft.EntityFrameworkCore.Storage;
using System.Diagnostics;

namespace AIAudioTalesServer.Data
{
    public class AppDbContext : DbContext
    {
        public DbSet<User> Users { get; set; }
        public DbSet<Book> Books { get; set; }
        public DbSet<PurchasedBooks> PurchasedBooks { get; set; }
        public DbSet<Story> Stories { get; set; }
        public DbSet<Part> Parts { get; set; }
        public DbSet<Answer> Answers { get; set; }
        public DbSet<RefreshToken> RefreshTokens { get; set; }
        public DbSet<SearchHistory> SearchHistories { get; set; }
        public DbSet<Category> BookCategories { get; set; }
        public DbSet<BasketItem> BasketItems { get; set; }

        public AppDbContext(DbContextOptions options) : base(options)
        {
            try
            {
                var databaseCreator = Database.GetService<IDatabaseCreator>() as RelationalDatabaseCreator;
                if(databaseCreator != null)
                {
                    if (!databaseCreator.CanConnect()) databaseCreator.Create();
                    if(!databaseCreator.HasTables()) databaseCreator.CreateTables();
                    DbSeederService seed = new DbSeederService(this);
                    seed.Seed();
                }
            }catch(Exception ex)
            {
                Console.WriteLine(ex);
            }
        }
        protected override void OnModelCreating(ModelBuilder modelBuilder)
        {
            modelBuilder.Entity<User>().HasKey(u => u.Id);
            modelBuilder.Entity<Book>().HasKey(b => b.Id);
            modelBuilder.Entity<Story>().HasKey(s => s.Id);
            modelBuilder.Entity<RefreshToken>().HasKey(t => t.UserId);
            modelBuilder.Entity<Category>().HasKey(c => c.Id);

            modelBuilder.Entity<SearchHistory>()
            .HasKey(sh => sh.Id)
            .IsClustered(false); // Remove clustered index from primary key

            modelBuilder.Entity<BasketItem>()
            .HasKey(bi => bi.Id)
            .IsClustered(false);

            modelBuilder.Entity<SearchHistory>()
             .HasIndex(sh => sh.UserId)
             .IsClustered(true)
             .IsUnique(false);
            // Add clustered index on UserId

            modelBuilder.Entity<BasketItem>()
             .HasIndex(bi => bi.UserId)
             .IsClustered(true)
             .IsUnique(false);
            // Add clustered index on UserId

            modelBuilder.Entity<User>()
            .HasIndex(u => u.Email) 
            .IsUnique();

            modelBuilder.Entity<PurchasedBooks>().HasKey(pb => new { pb.UserId, pb.BookId });

            modelBuilder.Entity<PurchasedBooks>()
            .HasOne<User>(pb => pb.User)
            .WithMany(u => u.PurchasedBooks)
            .HasForeignKey(pb => pb.UserId)
            .OnDelete(DeleteBehavior.NoAction);


            modelBuilder.Entity<PurchasedBooks>()
            .HasOne<Book>(pb => pb.Book)
            .WithMany(b => b.PurchasedBooks)
            .HasForeignKey(pb => pb.BookId)
            .OnDelete(DeleteBehavior.NoAction);


            modelBuilder.Entity<Story>()
           .HasOne<Book>(s => s.Book)
           .WithMany(b => b.Stories)
           .HasForeignKey(s => s.BookId)
           .OnDelete(DeleteBehavior.Cascade);

            modelBuilder.Entity<Book>()
            .HasOne<Category>(b => b.Category)
            .WithMany(c => c.BooksFromCategory)
            .HasForeignKey(b => b.CategoryId)
            .OnDelete(DeleteBehavior.NoAction);

            modelBuilder.Entity<Part>()
           .HasOne<Story>(p => p.Story)
           .WithMany(s => s.Parts)
           .HasForeignKey(p => p.StoryId)
           .OnDelete(DeleteBehavior.Cascade);

            modelBuilder.Entity<Answer>()
           .HasOne<Part>(a => a.CurrentPart)
           .WithMany(p => p.Answers)
           .HasForeignKey(a => a.CurrentPartId)
           .OnDelete(DeleteBehavior.Cascade);

            modelBuilder.Entity<Part>()
            .HasOne(p => p.ParentAnswer)
            .WithOne(a => a.NextPart)
            .HasForeignKey<Answer>(a => a.NextPartId)
            .OnDelete(DeleteBehavior.NoAction);

            modelBuilder.Entity<User>()
            .HasOne(u => u.RefreshToken)
            .WithOne(rt => rt.User)
            .HasForeignKey<RefreshToken>(rt => rt.UserId)
            .OnDelete(DeleteBehavior.Cascade);

            modelBuilder.Entity<Answer>()
            .Property(a => a.NextPartId)
            .IsRequired(false);

            modelBuilder.Entity<User>()
            .HasOne(u => u.SearchHistory)
            .WithOne(sh => sh.User)
            .HasForeignKey<SearchHistory>(sh => sh.UserId)
            .OnDelete(DeleteBehavior.Cascade);

            modelBuilder.Entity<BasketItem>()
           .HasOne<User>(bi => bi.User)
           .WithMany(u => u.BasketItems)
           .HasForeignKey(bi => bi.UserId)
           .OnDelete(DeleteBehavior.Cascade);

            modelBuilder.Entity<BasketItem>()
           .HasOne<Book>(bi => bi.Book)
           .WithMany(b => b.BasketItems)
           .HasForeignKey(bi => bi.BookId)
           .OnDelete(DeleteBehavior.Cascade);

            //Indexes
            modelBuilder.Entity<Book>()
                .HasIndex(b => b.Title);

            modelBuilder.Entity<Book>()
                .HasIndex(b => b.BookCategory);

        }

    }
}
