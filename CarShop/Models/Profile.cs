using SQLite;

namespace CarShop.Model
{
    public class Profile
    {
        [PrimaryKey, AutoIncrement]
        public int ProfileId { get; set; }
        public string Name { get; set; }
        public string Surname { get; set; }
        public string Email { get; set; }
        public string Bio { get; set; }
        public string PhoneNumber { get; set; }  // Additional field for contact
        public string Address { get; set; }      // Additional field for delivery
    }
}