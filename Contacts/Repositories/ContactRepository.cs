using Contacts.Data;
using Contacts.Models;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Diagnostics;

namespace Contacts.Repositories
{
    public interface IContactRepository
    {
        Task<IEnumerable<Contact>> GetListAsync();
        Task<Contact?> GetAsync(int id);
        Task CreateAsync(Contact contact);
        Task UpdateAsync(Contact contact);
        Task DeleteAsync(int id);
    }

    public class ContactRepository : IContactRepository
    {
        private readonly ApplicationDbContext _db;

        public ContactRepository(ApplicationDbContext db)
        {
            _db = db;
        }

        public async Task<IEnumerable<Contact>> GetListAsync()
        {
            return await _db.Contacts.Include(x=>x.Country).ToListAsync();
        }

        public async Task<Contact?> GetAsync(int id)
        {
            return await _db.Contacts.Include(x => x.Country).FirstOrDefaultAsync(u=>u.Id == id);
        }

        public async Task CreateAsync(Contact contact)
        {
            _db.Contacts.Add(contact);
            await _db.SaveChangesAsync();
        }

        public async Task UpdateAsync(Contact contact)
        {
            _db.Entry(contact).State = EntityState.Modified;
            await _db.SaveChangesAsync();
        }

        public async Task DeleteAsync(int id)
        {
            var contact = await GetAsync(id);

            if (contact != null)
            {
                _db.Contacts.Remove(contact);
                await _db.SaveChangesAsync();
            }
        }
    }
}
