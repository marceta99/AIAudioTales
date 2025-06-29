﻿using Kumadio.Domain.Entities;
using Kumadio.Infrastructure.Data.Seed.BooksPictures;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Infrastructure;
using Microsoft.EntityFrameworkCore.Storage;

namespace Kumadio.Infrastructure.Data
{
    public class AppDbContext : DbContext
    {
        public DbSet<User> Users { get; set; }
        public DbSet<Book> Books { get; set; }
        public DbSet<PurchasedBook> PurchasedBooks { get; set; }
        public DbSet<RefreshToken> RefreshTokens { get; set; }
        public DbSet<SearchHistory> SearchHistories { get; set; }
        public DbSet<Category> BookCategories { get; set; }
        public DbSet<Subscription> Subscriptions { get; set; }
        public DbSet<BookPart> BookParts { get; set; }
        public DbSet<Answer> Answers { get; set; }
        public DbSet<OnboardingQuestion> OnboardingQuestions { get; set; }
        public DbSet<OnboardingOption> OnboardingOptions { get; set; }
        public DbSet<OnboardingData> OnboardingData { get; set; }
        public DbSet<SelectedOption> SelectedOptions { get; set; }

        public AppDbContext(DbContextOptions options) : base(options)
        {
            try
            {
                var databaseCreator = Database.GetService<IDatabaseCreator>() as RelationalDatabaseCreator;
                if (databaseCreator != null)
                {
                    if (!databaseCreator.CanConnect()) databaseCreator.Create();
                    if (!databaseCreator.HasTables()) databaseCreator.CreateTables();
                    DbSeederService seed = new DbSeederService(this);
                    seed.Seed();
                }
            }
            catch (Exception ex)
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
           .HasOne(u => u.OnboardingData)
           .WithOne(s => s.User)
           .HasForeignKey<OnboardingData>(od => od.UserId)
           .OnDelete(DeleteBehavior.Cascade);

            #endregion

            #region Book

            modelBuilder.Entity<Book>().HasKey(b => b.Id);

            modelBuilder.Entity<Book>()
            .HasOne(b => b.Category)
            .WithMany(c => c.BooksFromCategory)
            .HasForeignKey(b => b.CategoryId)
            .OnDelete(DeleteBehavior.NoAction);

            modelBuilder.Entity<Book>()
            .HasOne(b => b.Creator)
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

            modelBuilder.Entity<SearchHistory>()
            .HasOne(sh => sh.User)
            .WithMany(u => u.SearchHistories)
            .HasForeignKey(sh => sh.UserId)
            .OnDelete(DeleteBehavior.NoAction);

            #endregion

            #region PurchasedBooks

            modelBuilder.Entity<PurchasedBook>().HasKey(pb => new { pb.UserId, pb.BookId });

            modelBuilder.Entity<PurchasedBook>()
            .HasOne(pb => pb.User)
            .WithMany(u => u.PurchasedBooks)
            .HasForeignKey(pb => pb.UserId)
            .OnDelete(DeleteBehavior.NoAction);


            modelBuilder.Entity<PurchasedBook>()
            .HasOne(pb => pb.Book)
            .WithMany(b => b.PurchasedBooks)
            .HasForeignKey(pb => pb.BookId)
            .OnDelete(DeleteBehavior.NoAction);

            modelBuilder.Entity<PurchasedBook>()
            .HasOne(pb => pb.PlayingPart)
            .WithMany(p => p.PurchasedBooks)
            .HasForeignKey(pb => pb.PlayingPartId)
            .OnDelete(DeleteBehavior.NoAction);

            #endregion

            #region RefreshToken

            modelBuilder.Entity<RefreshToken>().HasKey(t => t.UserId);

            #endregion

            #region BookParts

            modelBuilder.Entity<BookPart>().HasKey(bp => bp.Id);

            modelBuilder.Entity<BookPart>()
            .HasOne(bp => bp.Book)
            .WithMany(b => b.BookParts)
            .HasForeignKey(bp => bp.BookId)
            .OnDelete(DeleteBehavior.Cascade);

            #endregion

            #region Answers

            modelBuilder.Entity<Answer>().HasKey(a => a.Id);

            modelBuilder.Entity<Answer>()
            .HasOne(a => a.CurrentPart)
            .WithMany(bp => bp.Answers)
            .HasForeignKey(a => a.CurrentPartId)
            .OnDelete(DeleteBehavior.Cascade);

            modelBuilder.Entity<Answer>()
            .HasOne(a => a.NextPart)
            .WithOne(bp => bp.ParentAnswer)
            .HasForeignKey<Answer>(a => a.NextPartId)
            .OnDelete(DeleteBehavior.Restrict)
            .IsRequired(false);

            #endregion

            #region Fixed precision for decimal types

            modelBuilder.Entity<Book>()
                .Property(b => b.Price)
                .HasColumnType("decimal(18, 2)");

            modelBuilder.Entity<PurchasedBook>()
                .Property(p => p.PlayingPosition)
                .HasColumnType("decimal(18, 2)");

            #endregion

            #region Onboarding

            modelBuilder.Entity<OnboardingQuestion>().HasKey(ok => ok.Id);

            modelBuilder.Entity<OnboardingData>().HasKey(od => od.UserId);

            modelBuilder.Entity<OnboardingOption>().HasKey(op => op.Id);

            // 1:N Question → Options
            modelBuilder.Entity<OnboardingOption>()
                .HasOne(o => o.Question)
                .WithMany(q => q.Options)
                .HasForeignKey(o => o.QuestionId);

            // M:M OnboardingData → OnboardingOption
            modelBuilder.Entity<SelectedOption>().HasKey(so => new { so.OnboardingDataId, so.OnboardingOptionId });

            modelBuilder.Entity<SelectedOption>()
            .HasOne(so => so.OnboardingData)
            .WithMany(od => od.SelectedOptions)
            .HasForeignKey(so => so.OnboardingDataId)
            .OnDelete(DeleteBehavior.NoAction);

            modelBuilder.Entity<SelectedOption>()
            .HasOne(so => so.OnboardingOption)
            .WithMany(op => op.SelectedOptions)
            .HasForeignKey(so => so.OnboardingOptionId)
            .OnDelete(DeleteBehavior.NoAction);

            #endregion

        }
    }
}
