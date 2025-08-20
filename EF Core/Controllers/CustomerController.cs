using EF_Core.Models;
using EF_Core.DTOs;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using YourNamespace.Data;
using Microsoft.EntityFrameworkCore;
using EF_Core.Services.Interfaces;
using EF_Core.DTOs.For_Patch;

namespace EF_Core.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    public class CustomerController : ControllerBase
    {
        private ICustomerService _customerService;

        public CustomerController(ICustomerService customerService)
        {
            _customerService = customerService;
        }

        [HttpPost]
        public async Task<IActionResult> CreateCustomer([FromBody] CustomerDTO customerDto)
        {
            var customer = await _customerService.AddCustomerAsync(customerDto);
            if (customer == null)
            {
                return BadRequest($"Customer with CNIC {customerDto.Cnic} already exists.");
            }
            return CreatedAtAction("GetCustomerByCnic", new { cnic = customer.Cnic }, customer);
        }

        [HttpGet]
        public async Task<ActionResult<IEnumerable<Customer>>> GetAllCustomers(int limit = 2, int offset = 0)
        {
            var customers = await _customerService.GetAllCustomerAsync(limit, offset);
            return Ok(customers);
        }

        [HttpGet("{cnic}", Name = "GetCustomerByCnic")]
        public async Task<ActionResult<Customer>> GetCustomerByCnic(string cnic)
        {
            var customer = await _customerService.GetCustomerByCnicAsync(cnic);            
            if (customer == null)
            {
                return NotFound($"Customer with CNIC {cnic} not found.");
            }
            return Ok(customer);
        }

        [HttpPatch]
        public async Task<IActionResult> UpdateCustomer([FromBody] CustomerPDTO customerPdto)
        {
            if (customerPdto == null)
            {
                return BadRequest("Customer data is required.");
            }
            var existingCustomer = await _customerService.UpdateCustomerAsync(customerPdto);
            if (existingCustomer == null)
            {
                return NotFound($"Customer with Cnic {customerPdto.Cnic} not found.");
            }

            return Ok(existingCustomer);
        }

        [HttpDelete("{cnic}")]
        public async Task<IActionResult> DeleteCustomer(string cnic)
        {
            var customer = await _customerService.DeleteCustomerAsync(cnic);
            if (!customer)
            {
                return NotFound($"Customer with CNIC {cnic} not found.");
            }
            return Ok($"Customer with cnic {cnic} deleted successfully.");
        }
    }
}
