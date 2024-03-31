
using AIAudioTalesServer.Models;
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
            if (!_dbContext.Countries.Any())
            {
                var InitialData = new List<Country> {
                    new Country{ CountryName = "Serbia"},
                    new Country{ CountryName = "Germany"}
                };
                _dbContext.Countries.AddRange(InitialData);
                _dbContext.SaveChanges();
            }

            if (!_dbContext.Companies.Any())
            {
                var InitialDataCompanies = new List<Company> {
                    new Company{ CompanyName = "FIS" },
                    new Company{ CompanyName = "Seven Bridges"},
                };
                _dbContext.Companies.AddRange(InitialDataCompanies);
                _dbContext.SaveChanges();
            }

            if (!_dbContext.Jobs.Any())
            {
                var InitialDataJobs = new List<Job> {
                    new Job{ JobName = "Kuhinja", CompanyId = 1}
                };
                _dbContext.Jobs.AddRange(InitialDataJobs);
                _dbContext.SaveChanges();
            }
        }

       
    }
}
