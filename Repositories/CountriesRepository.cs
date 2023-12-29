using Entities;
using Microsoft.EntityFrameworkCore;
using RepositoryContracts;
using System.Diagnostics.Metrics;

namespace Repositories
{
    public class CountriesRepositor : ICountriesRepository
    {

        private readonly ApplicationDbContext _db;

        public CountriesRepositor(ApplicationDbContext db)
        {
            _db = db;
        }
        public async Task<Country> AddCountry(Country country)
        {
            _db.Countries.Add(country);
            await _db.SaveChangesAsync();
            return country;
        }

        public async Task<bool> DeleteCountry(Guid countryId)
        {
            _db.Countries.RemoveRange(_db.Countries.Where(temp => temp.CountryID == countryId));
            int rowDeleted = await _db.SaveChangesAsync();
            return rowDeleted > 0;
        }

        public async Task<List<Country>> GetAllCountries()
        {
           return await  _db.Countries.ToListAsync();
        }

        public async Task<Country> GetCountryById(Guid countryId)
        {
            return await  _db.Countries.FirstOrDefaultAsync(temp => temp.CountryID == countryId);
        }

        public async Task<Country> GetCountryByName(string countryName)
        {
           return await _db.Countries.FirstOrDefaultAsync(temp=>temp.CountryName == countryName);   
        }
    }
}