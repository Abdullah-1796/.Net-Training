namespace Sample.Models.Hotel_Management_System
{
    public class Employee
    {
        public Guid Eid { get; set; }
        public required string Cnic { get; set; }
        public required string Name { get; set; }
        public required string phone { get; set; }
        public required string email { get; set; }
        public required string role { get; set; }
    }
}