
using EF_Core.Models;
using EF_Core.DTOs;
using Microsoft.AspNetCore.Mvc;
using EF_Core.Services.Interfaces;
using EF_Core.DTOs.For_Patch;

namespace EF_Core.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    public class EmployeeController : ControllerBase
    {
        private IEmployeeService employeeService;

        public EmployeeController(IEmployeeService employeeService)
        {
            this.employeeService = employeeService;
        }

        [HttpPost]
        public async Task<IActionResult> CreateEmployee([FromBody] EmployeeDTO employee)
        {
            var emp = await employeeService.AddEmployeeAsync(employee);
            if (emp == null)
            {
                return BadRequest($"Employee with CNIC {employee.Cnic} already exists.");
            }
            return CreatedAtAction("GetEmployeeByCnic", new { cnic = emp.Cnic }, emp);
        }

        [HttpGet]
        public async Task<ActionResult<IEnumerable<Employee>>> GetAllEmployees(int limit = 2, int offset = 0)
        {
            var employees = await employeeService.GetAllEmployeesAsync(limit, offset);
            if (employees == null)
            {
                return NotFound("No employees found.");
            }
            return Ok(employees);
        }

        [HttpGet("{cnic}", Name = "GetEmployeeByCnic")]
        public async Task<ActionResult<Employee>> GetEmployeeByCnic(string cnic)
        {
            if (string.IsNullOrEmpty(cnic))
            {
                return BadRequest("Employee CNIC is required.");
            }
            var employee = await employeeService.GetEmployeeByCnicAsync(cnic);
            if (employee == null)
            {
                return NotFound($"Employee with CNIC {cnic} not found.");
            }
            return Ok(employee);
        }

        [HttpPatch]
        public async Task<IActionResult> UpdateEmployee([FromBody] EmployeePDTO employeePdto)
        {
            if (employeePdto == null)
            {
                return BadRequest("Employee data is required.");
            }
            var updatedEmployee = await employeeService.UpdateEmployeeAsync(employeePdto);
            if (updatedEmployee == null)
            {
                return NotFound($"Employee with cnic {employeePdto.Cnic} not found.");
            }
            return Ok(updatedEmployee);
        }

        [HttpDelete("cnic")]
        public async Task<IActionResult> DeleteEmployee(string cnic)
        {
            if (cnic == null)
            {
                return BadRequest("Employee cnic is required.");
            }
            var isDeleted = await employeeService.DeleteEmployeeAsync(cnic);
            if (!isDeleted)
            {
                return NotFound($"Employee with cnic {cnic} not found.");
            }
            return Ok($"Employee with cnic {cnic} deleted successfully.");
        }
    }
}
