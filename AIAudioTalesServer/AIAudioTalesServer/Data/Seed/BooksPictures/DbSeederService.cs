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
                        ImageURL= "https://d28hgpri8am2if.cloudfront.net/book_images/onix/cvr9781782493617/string-craft-9781782493617_hr.jpg"
                    },
                    new Book
                    {

                        BookCategory=BookCategory.Nature,
                        Title="Chronicles of Celestial Harmony",
                        Description="In a realm where music is magic, follow the journey of a gifted musician who must harness the celestial melodies to thwart an impending darkness",
                        ImageURL= "https://i.pinimg.com/736x/07/d4/8e/07d48e4f76e56efe05377eabc22da671.jpg"
                    },
                    new Book
                    {

                        BookCategory=BookCategory.Geography,
                        Title="Ephemeral Echoes",
                        Description="Unravel the mystery of a parallel dimension where echoes of past decisions create ripples in the fabric of reality. A mind-bending exploration of time and consequence.",
                        ImageURL= "https://covers.bookcoverzone.com/slir/w450/png24-front/bookcover0028822.jpg"
                    },
                    new Book
                    {

                        BookCategory=BookCategory.Math,
                        Title="The Alchemist's Legacy",
                        Description="A gripping adventure that follows an alchemist's apprentice in pursuit of a mythical artifact, rumored to hold the power to transmute the ordinary into the extraordinary.",
                        ImageURL= "https://s3.amazonaws.com/ASIDigitalAssets/00/00/84/94/09/Cover_l.gif"
                    },
                    new Book
                    {

                        BookCategory=BookCategory.History,
                        Title="Serenade of Shadows",
                        Description="Dive into the shadowy underworld of espionage and deceit, as a master spy navigates a web of intrigue to uncover a conspiracy threatening the stability of nations..",
                        ImageURL= "https://d28hgpri8am2if.cloudfront.net/book_images/onix/cvr9781781085677/string-city-9781781085677_hr.jpg"
                    },
                    new Book
                    {

                        BookCategory=BookCategory.BedTime,
                        Title="Cogs of Destiny",
                        Description="Set in a steampunk universe, this epic follows a group of unlikely heroes whose destinies intertwine as they race against time to prevent the activation of an ancient, world-altering machine.",
                        ImageURL= "https://encrypted-tbn0.gstatic.com/images?q=tbn:ANd9GcS48AmLhLGzaASe7UQdxHyjSbJRTtc0DakFiw&usqp=CAU"
                    },
                    new Book
                    {

                        BookCategory=BookCategory.Geography,
                        Title="Aetherial Odyssey",
                        Description="Embark on a fantastical journey through realms governed by elemental forces. A young elemental adept discovers their true potential while facing mythical creatures and ancient guardians.",
                        ImageURL= "https://images.booksense.com/images/286/402/9780316402286.jpg"
                    },
                    new Book
                    {

                        BookCategory=BookCategory.Nature,
                        Title="Aetherial Odyssey",
                        Description="Embark on a fantastical journey through realms governed by elemental forces. A young elemental adept discovers their true potential while facing mythical creatures and ancient guardians.",
                        ImageURL= "https://www.melissaaddey.com/wp-content/uploads/2018/10/Book-2_A-String-of-Silver-Beads_eBcov-small-scaled.jpg"
                    },
                    new Book
                    {

                        BookCategory=BookCategory.Nature,
                        Title="Nature 2",
                        Description="Embark on a fantastical journey through realms governed by elemental forces. A young elemental adept discovers their true potential while facing mythical creatures and ancient guardians.",
                        ImageURL= "https://images-na.ssl-images-amazon.com/images/S/compressed.photo.goodreads.com/books/1481162831i/28696598.jpg"
                    },
                    new Book
                    {

                        BookCategory=BookCategory.Nature,
                        Title="Nature 3",
                        Description="Embark on a fantastical journey through realms governed by elemental forces. A young elemental adept discovers their true potential while facing mythical creatures and ancient guardians.",
                        ImageURL= "https://encrypted-tbn0.gstatic.com/images?q=tbn:ANd9GcSJjPHr1W_CB1WXjqxBdu9aci2ANW5_BKw-Rw&usqp=CAU"
                    },
                    new Book
                    {

                        BookCategory=BookCategory.Nature,
                        Title="Nature 4",
                        Description="Embark on a fantastical journey through realms governed by elemental forces. A young elemental adept discovers their true potential while facing mythical creatures and ancient guardians.",
                        ImageURL= "https://m.media-amazon.com/images/W/MEDIAX_792452-T2/images/I/A1n8-0-wgYL._AC_UF350,350_QL80_.jpg"
                    },
                    new Book
                    {

                        BookCategory=BookCategory.Math,
                        Title="Math 1",
                        Description="Embark on a fantastical journey through realms governed by elemental forces. A young elemental adept discovers their true potential while facing mythical creatures and ancient guardians.",
                        ImageURL= "https://multiculturalchildrensbookday.com/wp-content/uploads/2018/07/thelengthofastring-mockup-1-325x500.png"
                    },
                    new Book
                    {

                        BookCategory=BookCategory.Math,
                        Title="Math 2",
                        Description="Embark on a fantastical journey through realms governed by elemental forces. A young elemental adept discovers their true potential while facing mythical creatures and ancient guardians.",
                        ImageURL= "https://dalemayer.com/cdn/shop/products/psych_0011_PV-StringOfTears_1800x1800.png?v=1692741054"
                    },
                    new Book
                    {

                        BookCategory=BookCategory.Math,
                        Title="Math 3",
                        Description="Embark on a fantastical journey through realms governed by elemental forces. A young elemental adept discovers their true potential while facing mythical creatures and ancient guardians.",
                        ImageURL= "https://images.squarespace-cdn.com/content/v1/63f648525c89981265aa8dbd/1677099626852-1CMNFX0GYDPR0UCAO15M/the-mitten-string-front-cover-lg.jpg"
                    },
                    new Book
                    {

                        BookCategory=BookCategory.Math,
                        Title="Math 4",
                        Description="Embark on a fantastical journey through realms governed by elemental forces. A young elemental adept discovers their true potential while facing mythical creatures and ancient guardians.",
                        ImageURL= "https://i.harperapps.com/hcanz/covers/9781911622260/y648.jpg"
                    },
                    new Book
                    {

                        BookCategory=BookCategory.Geography,
                        Title="Geography 1",
                        Description="Embark on a fantastical journey through realms governed by elemental forces. A young elemental adept discovers their true potential while facing mythical creatures and ancient guardians.",
                        ImageURL= "https://encrypted-tbn0.gstatic.com/images?q=tbn:ANd9GcSAfElIsHjYZ_7IktDAzoc2U_Ke9tTmRhD4Ig&usqp=CAU"
                    },
                    new Book
                    {

                        BookCategory=BookCategory.Geography,
                        Title="Geography 2",
                        Description="Embark on a fantastical journey through realms governed by elemental forces. A young elemental adept discovers their true potential while facing mythical creatures and ancient guardians.",
                        ImageURL= "https://encrypted-tbn0.gstatic.com/images?q=tbn:ANd9GcRpOuIrRtyB5TQFmCgt_oSvN3SYinGSR5uH5g&usqp=CAU"
                    },
                    new Book
                    {

                        BookCategory=BookCategory.Geography,
                        Title="Geography 3",
                        Description="Embark on a fantastical journey through realms governed by elemental forces. A young elemental adept discovers their true potential while facing mythical creatures and ancient guardians.",
                        ImageURL= "https://encrypted-tbn0.gstatic.com/images?q=tbn:ANd9GcQQxr1wVDs8H60Ry-MhWJwByaTzlePkjbi5dg&usqp=CAU"
                    },
                    new Book
                    {

                        BookCategory=BookCategory.Geography,
                        Title="Geography 4",
                        Description="Embark on a fantastical journey through realms governed by elemental forces. A young elemental adept discovers their true potential while facing mythical creatures and ancient guardians.",
                        ImageURL= "https://g.christianbook.com/g/slideshow/0/0735070/main/0735070_1_ftc.jpg"
                    },
                    new Book
                    {

                        BookCategory=BookCategory.History,
                        Title="Geography 1",
                        Description="Embark on a fantastical journey through realms governed by elemental forces. A young elemental adept discovers their true potential while facing mythical creatures and ancient guardians.",
                        ImageURL= "https://fictionophile.files.wordpress.com/2019/09/cl79.jpg"
                    },
                    new Book
                    {

                        BookCategory=BookCategory.History,
                        Title="Geography 2",
                        Description="Embark on a fantastical journey through realms governed by elemental forces. A young elemental adept discovers their true potential while facing mythical creatures and ancient guardians.",
                        ImageURL= "https://encrypted-tbn0.gstatic.com/images?q=tbn:ANd9GcTIFs7sAg6GHTGbL1bZbpTX_E09xI8vtGG0sg&usqp=CAU"
                    },
                    new Book
                    {

                        BookCategory=BookCategory.History,
                        Title="Geography 3",
                        Description="Embark on a fantastical journey through realms governed by elemental forces. A young elemental adept discovers their true potential while facing mythical creatures and ancient guardians.",
                        ImageURL= "https://www.jewishbookcouncil.org/sites/default/files/styles/book_cover_teal/public/images/length-of-a-string.jpg"
                    },
                    new Book
                    {

                        BookCategory=BookCategory.History,
                        Title="Geography 4",
                        Description="Embark on a fantastical journey through realms governed by elemental forces. A young elemental adept discovers their true potential while facing mythical creatures and ancient guardians.",
                        ImageURL= "https://encrypted-tbn0.gstatic.com/images?q=tbn:ANd9GcTXF6uULhSCTrUkXaycgDZbmFLY95eBTNQBcg&usqp=CAU"
                    },
                    new Book
                    {

                        BookCategory=BookCategory.BedTime,
                        Title="Bedtime 1",
                        Description="Embark on a fantastical journey through realms governed by elemental forces. A young elemental adept discovers their true potential while facing mythical creatures and ancient guardians.",
                        ImageURL= "https://www.psdcovers.com/wp-content/uploads/2022/12/HARDBOUND-BOOK-COVER-1419_1671469361188.jpg?x81780"
                    },
                    new Book
                    {

                        BookCategory=BookCategory.BedTime,
                        Title="Bedtime 2",
                        Description="Embark on a fantastical journey through realms governed by elemental forces. A young elemental adept discovers their true potential while facing mythical creatures and ancient guardians.",
                        ImageURL= "https://cardinalrulepress.com/cdn/shop/products/9781733035989_1600x.jpg?v=1645197367"
                    },
                    new Book
                    {

                        BookCategory=BookCategory.BedTime,
                        Title="Bedtime 3",
                        Description="Embark on a fantastical journey through realms governed by elemental forces. A young elemental adept discovers their true potential while facing mythical creatures and ancient guardians.",
                        ImageURL= "https://i.ebayimg.com/images/g/8p0AAOSwoCdgz86N/s-l1600.jpg"
                    },
                    new Book
                    {

                        BookCategory=BookCategory.BedTime,
                        Title="Bedtime 4",
                        Description="Embark on a fantastical journey through realms governed by elemental forces. A young elemental adept discovers their true potential while facing mythical creatures and ancient guardians.",
                        ImageURL= "https://encrypted-tbn0.gstatic.com/images?q=tbn:ANd9GcT_8hf-iCs_E-2QUKRll8AtUPTpYzrvQZKa5U6uUwkMouQVyNRmuTwaiXxMwxJ6QQfWfHU&usqp=CAU"
                    },
                    new Book
                    {

                        BookCategory=BookCategory.BedTime,
                        Title="Aetherial Odyssey",
                        Description="Embark on a fantastical journey through realms governed by elemental forces. A young elemental adept discovers their true potential while facing mythical creatures and ancient guardians.",
                        ImageURL= "https://www.melissaaddey.com/wp-content/uploads/2018/10/Book-2_A-String-of-Silver-Beads_eBcov-small-scaled.jpg"
                    },
                    new Book
                    {

                        BookCategory=BookCategory.BedTime,
                        Title="Nature 2",
                        Description="Embark on a fantastical journey through realms governed by elemental forces. A young elemental adept discovers their true potential while facing mythical creatures and ancient guardians.",
                        ImageURL= "https://images-na.ssl-images-amazon.com/images/S/compressed.photo.goodreads.com/books/1481162831i/28696598.jpg"
                    },
                    new Book
                    {

                        BookCategory=BookCategory.BedTime,
                        Title="Nature 3",
                        Description="Embark on a fantastical journey through realms governed by elemental forces. A young elemental adept discovers their true potential while facing mythical creatures and ancient guardians.",
                        ImageURL= "https://encrypted-tbn0.gstatic.com/images?q=tbn:ANd9GcSJjPHr1W_CB1WXjqxBdu9aci2ANW5_BKw-Rw&usqp=CAU"
                    },
                    new Book
                    {

                        BookCategory=BookCategory.BedTime,
                        Title="Nature 4",
                        Description="Embark on a fantastical journey through realms governed by elemental forces. A young elemental adept discovers their true potential while facing mythical creatures and ancient guardians.",
                        ImageURL= "https://m.media-amazon.com/images/W/MEDIAX_792452-T2/images/I/A1n8-0-wgYL._AC_UF350,350_QL80_.jpg"
                    },
                    new Book
                    {

                        BookCategory=BookCategory.BedTime,
                        Title="Math 1",
                        Description="Embark on a fantastical journey through realms governed by elemental forces. A young elemental adept discovers their true potential while facing mythical creatures and ancient guardians.",
                        ImageURL= "https://multiculturalchildrensbookday.com/wp-content/uploads/2018/07/thelengthofastring-mockup-1-325x500.png"
                    },
                    new Book
                    {

                        BookCategory=BookCategory.BedTime,
                        Title="Math 2",
                        Description="Embark on a fantastical journey through realms governed by elemental forces. A young elemental adept discovers their true potential while facing mythical creatures and ancient guardians.",
                        ImageURL= "https://dalemayer.com/cdn/shop/products/psych_0011_PV-StringOfTears_1800x1800.png?v=1692741054"
                    },
                    new Book
                    {

                        BookCategory=BookCategory.BedTime,
                        Title="Math 3",
                        Description="Embark on a fantastical journey through realms governed by elemental forces. A young elemental adept discovers their true potential while facing mythical creatures and ancient guardians.",
                        ImageURL= "https://images.squarespace-cdn.com/content/v1/63f648525c89981265aa8dbd/1677099626852-1CMNFX0GYDPR0UCAO15M/the-mitten-string-front-cover-lg.jpg"
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
