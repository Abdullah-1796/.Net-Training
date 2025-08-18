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
    public class EmployeeController : ControllerBase
    {
        private AppDbContext context;

        public EmployeeController(AppDbContext appDbContext)
        {
            context = appDbContext;
        }

        [HttpPost]
        public async Task<IActionResult> CreateEmployee([FromBody] Employee employee)
        {
            if (employee == null)
            {
                return BadRequest("Employee data is required.");
            }
            
            context.Employees.Add(employee);
            await context.SaveChangesAsync();
            return CreatedAtAction(nameof(CreateEmployee), new { id = employee.EmployeeId }, employee);
        }

        [HttpGet]
        public ActionResult<IEnumerable<Employee>> GetEmployees(int limit = 2, int offset = 0)
        {
            var employees = context.Employees.ToList().Take(limit).Skip(offset);
            return Ok(employees);
        }

        [HttpGet("{cnic}")]
        public ActionResult<Employee> GetEmployeeByCnic(string cnic)
        {
            Employee? employee = context.Employees.FirstOrDefault(e => e.Cnic == cnic);
            
            if (employee == null)
            {
                return NotFound($"Employee with CNIC {cnic} not found.");
            }
            return Ok(employee);
        }

        [HttpPatch]
        public async Task<IActionResult> UpdateEmployee([FromBody] EmployeeDTO employee)
        {
            if (employee == null)
            {
                return BadRequest("Employee data is required.");
            }
            var existingEmployee = await context.Employees.FirstOrDefaultAsync(e => e.Cnic == employee.Cnic);
            if (existingEmployee == null)
            {
                return NotFound($"Employee with ID {employee.EmployeeId} not found.");
            }

            existingEmployee.Name = employee.Name ?? existingEmployee.Name;
            existingEmployee.Phone = employee.Phone ?? existingEmployee.Phone;
            existingEmployee.Email = employee.Email ?? existingEmployee.Email;
            existingEmployee.Role = employee.Role ?? existingEmployee.Role;
            await context.SaveChangesAsync();
            return NoContent();
        }

        [HttpDelete("cnic")]
        public async Task<IActionResult> DeleteEmployee(string cnic)
        {
            if (cnic == null)
            {
                return BadRequest("Employee data is required.");
            }
            var existingEmployee = await context.Employees.FirstOrDefaultAsync(e => e.Cnic == cnic);
            if (existingEmployee == null)
            {
                return NotFound($"Employee with cnic {cnic} not found.");
            }
            context.Employees.Remove(existingEmployee);
            await context.SaveChangesAsync();
            return NoContent();
        }
    }
}
