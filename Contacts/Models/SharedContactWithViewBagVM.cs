using Microsoft.Extensions.Configuration.UserSecrets;

namespace Contacts.Models
{
    public class SharedContactWithViewBagVM
    {
        public int Id { get; set; }
        public string SelectedUserId { get; set; }
    }
}
