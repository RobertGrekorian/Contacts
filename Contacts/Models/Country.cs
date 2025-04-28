using System.ComponentModel.DataAnnotations;

namespace Contacts.Models
{
    public class Country
    {
        [Key]
        public int CountryId { get; set; } 
        public string? CountryName { get; set; }
    }
}
