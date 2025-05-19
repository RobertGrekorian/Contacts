using Microsoft.AspNetCore.Mvc.ModelBinding.Validation;
using Microsoft.AspNetCore.Mvc.Rendering;

namespace ContactsData.Models
{
    public class ContactVM
    {
        public Contact? Contact {  get; set; }
        [ValidateNever]
        public IEnumerable<SelectListItem>? CountryList { get; set; }
    }

    public class SharedContactVM
    {
        public int Id { get; set; }
        public string SelectedUserId { get; set; }
        public IEnumerable<SelectListItem>? Users { get; set; }
        
    }
}
