using ContactsData.Models;
using ContactsData.Data;
using Microsoft.EntityFrameworkCore;

namespace Contacts.Repositories
{
    public interface ICountryRepository
    {
        Task<IEnumerable<Country>> GetListAsync();

        Task<Country> GetByIdAsync(int id);
        Task CreateAsync(Country country);
        Task UpdateAsync(Country country);
        Task DeleteAsync(int id);
    }
    public class CountryRepository : ICountryRepository
    {
        private readonly ApplicationDbContext _db;
        public CountryRepository(ApplicationDbContext db)
        {
            _db = db;
        }

        public async Task<IEnumerable<Country>> GetListAsync()
        {
            List<Country> country = await _db.Countries.ToListAsync();
            return country;
        }

        public async Task<Country?> GetByIdAsync(int id)
        {
            return await _db.Countries.FirstOrDefaultAsync(u=> u.CountryId == id);                        
        }

        public async Task CreateAsync(Country country)
        {
            _db.Countries.Add(country);
            await _db.SaveChangesAsync();            
        }
        public async Task UpdateAsync(Country country)
        {
            _db.Entry(country).State = EntityState.Modified;
            await _db.SaveChangesAsync();
        }
        public async Task DeleteAsync(int id)
        {            
            var country = await GetByIdAsync(id);
            if(country != null)
            _db.Countries.Remove(country);
            await _db.SaveChangesAsync();
        }
    }
}
