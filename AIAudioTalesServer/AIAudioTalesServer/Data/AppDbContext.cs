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
        public DbSet<RefreshToken> RefreshTokens { get; set; }
        public DbSet<SearchHistory> SearchHistories { get; set; }
        public DbSet<Category> BookCategories { get; set; }
        public DbSet<BasketItem> BasketItems { get; set; }
        public DbSet<Subscription> Subscriptions { get; set; }
        public DbSet<BookPart> BookParts { get; set; }
        public DbSet<Answer> Answers { get; set; }

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
            #region User
            
            modelBuilder.Entity<User>().HasKey(u => u.Id);

            // Add clustered index on UserId
            modelBuilder.Entity<User>()
            .HasIndex(u => u.Email)
            .IsUnique();

            modelBuilder.Entity<User>()
            .HasOne(u => u.RefreshToken)
            .WithOne(rt => rt.User)
            .HasForeignKey<RefreshToken>(rt => rt.UserId)
            .OnDelete(DeleteBehavior.Cascade);

            modelBuilder.Entity<User>()
            .HasOne(u => u.Subscription)
            .WithOne(s => s.User)
            .HasForeignKey<Subscription>(s => s.UserId)
            .OnDelete(DeleteBehavior.Cascade);

            modelBuilder.Entity<User>()
            .HasOne(u => u.SearchHistory)
            .WithOne(sh => sh.User)
            .HasForeignKey<SearchHistory>(sh => sh.UserId)
            .OnDelete(DeleteBehavior.Cascade);
            
            #endregion

            #region Book
            
            modelBuilder.Entity<Book>().HasKey(b => b.Id);

            modelBuilder.Entity<Book>()
            .HasOne<Category>(b => b.Category)
            .WithMany(c => c.BooksFromCategory)
            .HasForeignKey(b => b.CategoryId)
            .OnDelete(DeleteBehavior.NoAction);

            modelBuilder.Entity<Book>()
            .HasOne<User>(b => b.Creator)
            .WithMany(u => u.CreatedBooks)
            .HasForeignKey(b => b.CreatorId)
            .OnDelete(DeleteBehavior.NoAction);

            //Indexes
            modelBuilder.Entity<Book>()
                .HasIndex(b => b.Title); // nisam siguran da mi treba ovaj title index za books ali mogu da razmilim o tome kasnije, nzm ni sto sam ga dodao pre

            modelBuilder.Entity<Book>()
                .HasIndex(b => b.CategoryId);
            
            #endregion

            #region Category
            
            modelBuilder.Entity<Category>().HasKey(c => c.Id);

            modelBuilder.Entity<Category>()
            .Property(c => c.Id)
            .ValueGeneratedNever(); // Indicates the id generated manualy, and not automaticaly with Identity_Insert
            
            #endregion

            #region SearchHistory
            
            modelBuilder.Entity<SearchHistory>()
           .HasKey(sh => sh.Id)
           .IsClustered(false); // Remove clustered index from primary key

            modelBuilder.Entity<SearchHistory>()
             .HasIndex(sh => sh.UserId)
             .IsClustered(true)
             .IsUnique(false);

            #endregion

            #region BasketItem

            modelBuilder.Entity<BasketItem>()
            .HasKey(bi => bi.Id)
            .IsClustered(false);

            modelBuilder.Entity<BasketItem>()
             .HasIndex(bi => bi.UserId)
             .IsClustered(true)
             .IsUnique(false);

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

            #endregion

            #region PurchasedBooks

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

            #endregion

            #region RefreshToken

            modelBuilder.Entity<RefreshToken>().HasKey(t => t.UserId);

            #endregion

            #region BookParts

            modelBuilder.Entity<BookPart>().HasKey(bp => bp.Id);

            modelBuilder.Entity<BookPart>()
            .HasOne<Book>(bp => bp.Book)
            .WithMany(b => b.BookParts)
            .HasForeignKey(bp => bp.BookId)
            .OnDelete(DeleteBehavior.Cascade);

            #endregion

            #region Answers

            modelBuilder.Entity<Answer>().HasKey(a => a.Id);

            modelBuilder.Entity<Answer>()
            .HasOne<BookPart>(a => a.CurrentPart)
            .WithMany(bp => bp.Answers)
            .HasForeignKey(a => a.CurrentPartId)
            .OnDelete(DeleteBehavior.Cascade);

            modelBuilder.Entity<Answer>()
            .HasOne<BookPart>(a => a.NextPart)
            .WithOne(bp => bp.ParentAnswer)
            .HasForeignKey<Answer>(a => a.NextPartId)
            .OnDelete(DeleteBehavior.Restrict)
            .IsRequired(false);

            #endregion

        }

    }
}
