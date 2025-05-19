using ContactsData.Models;
using Microsoft.EntityFrameworkCore;
using Microsoft.AspNetCore.Identity.EntityFrameworkCore;

namespace ContactsData.Data
{
    public class ApplicationDbContext : IdentityDbContext<ApplicationUser> // instead of DBContext because we are using Identity and 
                                                                           // giving the custom ApplicationUser as the user model 
    {
        public ApplicationDbContext(DbContextOptions<ApplicationDbContext> options) : base(options)
        {

        }
        public DbSet<Contact> Contacts { get; set; }
        public DbSet<Country> Countries { get; set; }
        public DbSet<SharedContact> SharedContacts { get; set; }

        protected override void OnModelCreating(ModelBuilder builder)
        {
            base.OnModelCreating(builder);


            /*
             * DeleteBehavior is defining what is the policy on contact deleting.
             * Restrict: You can't delete a shared contact and at first you should delete that relationship and unshare it
             * Cascade: All records related to that contact in this table will be deleted by deleting that contact
            */
            builder.Entity<SharedContact>()
                .HasOne(sc => sc.Contact)
                .WithMany(c => c.SharedWithUsers)
                .HasForeignKey(sc => sc.Contactid)
                .OnDelete(DeleteBehavior.Restrict);
        }
    }
}
