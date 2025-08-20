using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace EF_Core.DTOs
{
    public class CustomerDTO
    {
        public required string Cnic { get; set; }

        public required string Name { get; set; }

        public required string Phone { get; set; }

        public required string Email { get; set; }
    }
}
