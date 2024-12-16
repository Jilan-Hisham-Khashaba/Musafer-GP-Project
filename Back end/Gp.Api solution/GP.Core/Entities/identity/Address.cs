using System.Runtime;

namespace GP.core.Entities.identity
{
    public class Address
    {
        public int Id { get; set; }

        public string FirstName { get; set; }

        public string LastName { get; set; }

        public string? Street { get; set; }

        public string City { get; set; }

        public string Country { get; set; }

        public string? NationalId { get; set; }

        public string AppUserId { get; set; } //foreign key
        public AppUser User { get; set; } //navigational prop =>one



    }
}