
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
            // Seed the database with initial data

        }

       
    }
}
