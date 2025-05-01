using Contacts.Models;
using Microsoft.EntityFrameworkCore;
using Microsoft.AspNetCore.Identity.EntityFrameworkCore;

namespace Contacts.Data
{
    public class ApplicationDbContext : IdentityDbContext<ApplicationUser> // instead of DBContext because we are using Identity and 
                                                                           // giving the custom ApplicationUser as the user model 
    {
        public ApplicationDbContext(DbContextOptions<ApplicationDbContext> options) : base(options) { 
        
        }
        public DbSet<Contact> Contacts { get; set; }
        public DbSet<Country> Countries { get; set; }
    }
}
