﻿using System.ComponentModel.DataAnnotations;

namespace ContactsData.Models
{
    public class Country
    {
        [Key]
        public int CountryId { get; set; } 
        public string? CountryName { get; set; }
    }
}
