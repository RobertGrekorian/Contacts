using ContactsData.Data;
using Contacts.Migrations;
using ContactsData.Models;
using Microsoft.AspNetCore.Identity;
using Microsoft.EntityFrameworkCore;
using System.Security.Claims;

namespace Contacts.Repositories
{
    public interface ISharedContactRepository 
    {
        Task ShareContactAsync(int contactId,  string targetUserId, bool canEdit);
        Task UnshareContactAsync(int contactId,  string targetUserId);
        Task<List<SharedContact>> GetSharedContactsAsync();
        Task<List<SharedContact>> GetContactsSharedWithMeAsync();
    }

    public class SharedContactRepository : ISharedContactRepository
    {
        private readonly ApplicationDbContext _db;
        private readonly IHttpContextAccessor _httpContextAccessor;
        private readonly UserManager<ApplicationUser> _userManager;
        private readonly string _userId;
        public SharedContactRepository(ApplicationDbContext db, IHttpContextAccessor httpContextAccessor, UserManager<ApplicationUser> userManager)
        {
            _db = db;
            _httpContextAccessor = httpContextAccessor;
            _userManager = userManager;
            _userId = _httpContextAccessor.HttpContext?.User?.FindFirstValue(ClaimTypes.NameIdentifier);
        }
        public async Task<List<SharedContact>> GetContactsSharedWithMeAsync() {
            return await _db.SharedContacts.Where(u => u.Userid == _userId).Include(x => x.Contact).ToListAsync();
        }

        public async Task<List<SharedContact>> GetSharedContactsAsync() { 
            return await _db.SharedContacts.ToListAsync();
        }

        public async Task ShareContactAsync(int contactId, string targetUserId, bool canEdit)
        {
            var contact = await _db.Contacts.FirstOrDefaultAsync(x => x.Id == contactId && x.UserId == _userId);

            if (contact == null)
            {
                throw new Exception("Contact is not exists");
            }

            var targetUser = await _userManager.FindByIdAsync(targetUserId);
            if (targetUser == null)
            {
                throw new Exception("Target user is not exists");
            }

            var existsingShare = await _db.SharedContacts.FirstOrDefaultAsync(x => x.Contactid == contactId && x.Userid == targetUserId);

            if(existsingShare != null)
            {
                existsingShare.CanEdit = canEdit;
            } else
            {
                _db.SharedContacts.Add(new SharedContact
                {
                    Userid = targetUserId,
                    Contactid = contactId,
                    CanEdit = canEdit,
                });
            }
            await _db.SaveChangesAsync();
        }

        public async Task UnshareContactAsync(int contactId, string targetUserId)
        {
            if(! await _db.Contacts.Where(u=>u.Id == contactId && u.UserId == _userId).AnyAsync())
            {
                throw new Exception("Contact Not Found");
               
            }
            if ((await _userManager.FindByIdAsync(targetUserId)) == null)
            {
                throw new Exception("Target User Not Found");
            }

            var SharedContactsToUnshare =  _db.SharedContacts.Where(u => u.Contactid == contactId &&
                                            (u.Userid == targetUserId)).ToList();
            if(SharedContactsToUnshare != null)
             _db.SharedContacts.RemoveRange(SharedContactsToUnshare);
            await _db.SaveChangesAsync();
        }
    }
}
