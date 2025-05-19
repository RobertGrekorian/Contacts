namespace ContactsData.Models
{
    public class SharedContact
    {
        public int Id { get; set; }
        public int Contactid { get; set; }
        public Contact? Contact { get; set; }
        public string Userid { get; set; }
        public ApplicationUser? User { get; set; }
        public DateTime ShareDate { get; set; } = DateTime.Now;
        public bool CanEdit { get; set; } = true;
    }
}
 