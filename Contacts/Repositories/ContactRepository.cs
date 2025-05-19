using ContactsData.Models;
using ContactsData.Data;
using Microsoft.AspNetCore.Identity;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Diagnostics;
using System.Security.Claims;

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
        private readonly SignInManager<ApplicationUser> _signInManager;
        private readonly IHttpContextAccessor _httpContextAccessor;
        private readonly string _userId;
        public ContactRepository(ApplicationDbContext db,SignInManager<ApplicationUser> signInManager, IHttpContextAccessor httpContextAccessor)
        {
            _db = db;
            _signInManager = signInManager;
            _httpContextAccessor = httpContextAccessor;
            _userId = _httpContextAccessor.HttpContext?.User?.FindFirstValue(ClaimTypes.NameIdentifier);
        }

        public async Task<IEnumerable<Contact>> GetListAsync()
        {            
            return await _db.Contacts.Include(x=>x.Country).Where(u=>u.UserId==_userId).ToListAsync();
        }

        public async Task<Contact?> GetAsync(int id)
        {
            return await _db.Contacts.Include(x => x.Country).FirstOrDefaultAsync(u=>u.Id == id && u.UserId == _userId);
        }

        public async Task CreateAsync(Contact contact)
        {            
            if (_userId != null)
                contact.UserId = _userId;
            _db.Contacts.Add(contact);
            await _db.SaveChangesAsync();
        }

        public async Task UpdateAsync(Contact contact)
        {
            Contact _contact = await GetAsync(contact.Id);
            _contact.Email = contact.Email;
            _contact.FirstName = contact.FirstName;
            _contact.LastName = contact.LastName;
            _contact.Phone = contact.Phone;
            _contact.City = contact.City;
            _contact.State = contact.State;
            _contact.Address = contact.Address;
            _contact.CountryId = contact.CountryId;

            
            if (_contact.UserId == _userId)
            {
                _db.Update(_contact);
                await _db.SaveChangesAsync();
            }
            // Reymond's solution passing userid to repo and check with  contact.userid
            //if (contact.UserId != _userId)
            //{
            //    throw  new UnauthorizedAccessException("User not authorized to update this contact");
            //}

            //_db.Entry(contact).State = EntityState.Modified;
            //await _db.SaveChangesAsync();
        }

        public async Task DeleteAsync(int id)
        {
            var contact = await GetAsync(id);
            

            if (contact != null)
            {
                if (contact.UserId == _userId)
                {
                    _db.Contacts.Remove(contact);
                    await _db.SaveChangesAsync();
                }
            }
        }
    }
}
