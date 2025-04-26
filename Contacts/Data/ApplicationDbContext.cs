using Contacts.Models;
using Microsoft.EntityFrameworkCore;

namespace Contacts.Data
{
    public class ApplicationDbContext: DbContext
    {
        public ApplicationDbContext(DbContextOptions<ApplicationDbContext> options) : base(options) { 
        
        }
        public DbSet<Contact> Contacts { get; set; }
        public DbSet<Country> Countries { get; set; }
    }
}
