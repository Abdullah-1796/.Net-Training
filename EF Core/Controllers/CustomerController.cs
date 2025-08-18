using EF_Core.Models;
using EF_Core.DTOs;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using YourNamespace.Data;
using Microsoft.EntityFrameworkCore;

namespace EF_Core.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    public class CustomerController : ControllerBase
    {
        private AppDbContext context;
        public CustomerController(AppDbContext appDbContext)
        {
            context = appDbContext;
        }

        [HttpPost]
        public async Task<IActionResult> CreateCustomer([FromBody] Customer customer)
        {
            if (customer == null)
            {
                return BadRequest("Customer data is required.");
            }
            
            context.Customers.Add(customer);
            await context.SaveChangesAsync();

            return CreatedAtAction(nameof(CreateCustomer), new { id = customer.CustomerId }, customer);
        }

        [HttpGet]
        public ActionResult<IEnumerable<Customer>> GetAllCustomers(int limit = 2, int offset = 0)
        {
            var customers = context.Customers.OrderBy(c => c.CustomerId).ToList().Take(limit).Skip(offset);
            return Ok(customers);
        }

        [HttpGet("{cnic}")]
        public ActionResult<Customer> GetCustomerById(string cnic)
        {
            Customer? customer = context.Customers.Where(c => c.Cnic == cnic).FirstOrDefault();
            
            if (customer == null)
            {
                return NotFound($"Customer with CNIC {cnic} not found.");
            }
            return Ok(customer);
        }

        [HttpPatch]
        public async Task<IActionResult> UpdateCustomer([FromBody] CustomerDTO customer)
        {
            if (customer == null)
            {
                return BadRequest("Customer data is required.");
            }
            var existingCustomer = await context.Customers.FirstOrDefaultAsync(c => c.Cnic == customer.Cnic);
            if (existingCustomer == null)
            {
                return NotFound($"Customer with ID {customer.CustomerId} not found.");
            }
            existingCustomer.Name = customer.Name ?? existingCustomer.Name;
            existingCustomer.Phone = customer.Phone ?? existingCustomer.Phone;
            existingCustomer.Email = customer.Email ?? existingCustomer.Email;
            await context.SaveChangesAsync();
            return NoContent();
        }

        [HttpDelete("{cnic}")]
        public async Task<IActionResult> DeleteCustomer(string cnic)
        {
            var customer = await context.Customers.FirstOrDefaultAsync(c => c.Cnic == cnic);
            if (customer == null)
            {
                return NotFound($"Customer with CNIC {cnic} not found.");
            }
            context.Customers.Remove(customer);
            await context.SaveChangesAsync();
            return NoContent();
        }
    }
}
