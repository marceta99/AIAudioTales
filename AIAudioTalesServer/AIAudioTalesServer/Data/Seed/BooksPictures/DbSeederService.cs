using AIAudioTalesServer.Models;
using AIAudioTalesServer.Models.Enums;
using Microsoft.EntityFrameworkCore;
using System.Xml;

namespace AIAudioTalesServer.Data.Seed.BooksPictures
{
    public class DbSeederService
    {
        private readonly AppDbContext _dbContext;

        public DbSeederService(AppDbContext dbContext)
        {
            _dbContext = dbContext;
        }

        public void Seed()
        {
            // Check if the database is empty
            if (!_dbContext.Books.Any())
            {
                // Seed the database with initial data
                var initialData = new List<Book>
                {
                    new Book
                    {

                        BookCategory=BookCategory.Nature,
                        Title="The Little Farm Tales",
                        Description="A tale of lost magic and ancient secrets, where a young hero embarks on a quest to rediscover the forgotten powers that can reshape the fate of the world",
                        ImageData= GetPictureBytes("Farm.png")
                    },
                    new Book
                    {

                        BookCategory=BookCategory.Nature,
                        Title="Chronicles of Celestial Harmony",
                        Description="In a realm where music is magic, follow the journey of a gifted musician who must harness the celestial melodies to thwart an impending darkness",
                        ImageData= GetPictureBytes("Africa.png")
                    },
                    new Book
                    {

                        BookCategory=BookCategory.Geography,
                        Title="Ephemeral Echoes",
                        Description="Unravel the mystery of a parallel dimension where echoes of past decisions create ripples in the fabric of reality. A mind-bending exploration of time and consequence.",
                        ImageData= GetPictureBytes("Book10.jpg")
                    },
                    new Book
                    {

                        BookCategory=BookCategory.Math,
                        Title="The Alchemist's Legacy",
                        Description="A gripping adventure that follows an alchemist's apprentice in pursuit of a mythical artifact, rumored to hold the power to transmute the ordinary into the extraordinary.",
                        ImageData= GetPictureBytes("Book11.jpg")
                    },
                    new Book
                    {

                        BookCategory=BookCategory.History,
                        Title="Serenade of Shadows",
                        Description="Dive into the shadowy underworld of espionage and deceit, as a master spy navigates a web of intrigue to uncover a conspiracy threatening the stability of nations..",
                        ImageData= GetPictureBytes("Book12.jpg")
                    },
                    new Book
                    {

                        BookCategory=BookCategory.BedTime,
                        Title="Cogs of Destiny",
                        Description="Set in a steampunk universe, this epic follows a group of unlikely heroes whose destinies intertwine as they race against time to prevent the activation of an ancient, world-altering machine.",
                        ImageData= GetPictureBytes("Book15.jpg")
                    },
                    new Book
                    {

                        BookCategory=BookCategory.Geography,
                        Title="Aetherial Odyssey",
                        Description="Embark on a fantastical journey through realms governed by elemental forces. A young elemental adept discovers their true potential while facing mythical creatures and ancient guardians.",
                        ImageData= GetPictureBytes("Book16.jpg")
                    },

            };

                _dbContext.Books.AddRange(initialData);
                _dbContext.SaveChanges();
            }
        }

        private byte[] GetPictureBytes(string imageName)
        {
            // Construct the full path to the picture
            var imagePath = "Data/Seed/BooksPictures/" + imageName;

            // Load the picture from the specified path and convert it to byte[]
            try
            {
                return File.ReadAllBytes(imagePath);
            }
            catch (Exception ex)
            {
                // Handle exceptions (e.g., file not found)
                Console.WriteLine($"Error loading picture: {ex.Message}");
                return null;
            }
        }
    }
}
