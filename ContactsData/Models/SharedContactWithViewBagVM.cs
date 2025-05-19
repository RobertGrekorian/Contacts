using Microsoft.Extensions.Configuration.UserSecrets;

namespace ContactsData.Models
{
    public class SharedContactWithViewBagVM
    {
        public int Id { get; set; }
        public string? SelectedUserId { get; set; }
    }
}
