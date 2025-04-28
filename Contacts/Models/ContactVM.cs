using Microsoft.AspNetCore.Mvc.ModelBinding.Validation;
using Microsoft.AspNetCore.Mvc.Rendering;

namespace Contacts.Models
{
    public class ContactVM
    {
        public Contact? Contact {  get; set; }
        [ValidateNever]
        public IEnumerable<SelectListItem>? CountryList { get; set; }
    }
}
