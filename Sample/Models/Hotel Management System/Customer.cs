namespace Sample.Models.Hotel_Management_System
{
    public class Customer
    {
        public Guid Cid { get; set; }
        public required string Cnic { get; set; }
        public required string Name { get; set; }
        public required string Phone { get; set; }
        public required string Email { get; set; }
    }
}
