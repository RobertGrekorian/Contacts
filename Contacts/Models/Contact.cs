using Microsoft.AspNetCore.Mvc.ModelBinding.Validation;
using Microsoft.EntityFrameworkCore;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace Contacts.Models
{
    [Index(nameof(Email), IsUnique = true)]
    public class Contact
    {
        public int Id { get; set; }
        public string? UserId { get;set; }
        public string? FirstName { get; set; }
        public string? LastName { get; set; }
        [Required]
        [EmailAddress]
        public string Email { get; set; }
        public string? Phone { get; set; }
        public string? Address { get; set; }
        public string? City { get; set; }
        public string? State { get; set; }
        public string? PostalCode { get; set; }        
        public int CountryId { get; set; }
        [ForeignKey("CountryId")]
        [ValidateNever]
        public Country Country { get; set; }
        public ICollection<SharedContact>? SharedWithUsers { get; set; }

    }
}
