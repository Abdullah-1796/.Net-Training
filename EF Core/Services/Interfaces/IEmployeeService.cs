using EF_Core.DTOs;
using EF_Core.DTOs.For_Patch;
namespace EF_Core.Services.Interfaces
{
    public interface IEmployeeService
    {
        Task<IEnumerable<EmployeeDTO?>> GetAllEmployeesAsync(int limit = 2, int offset = 0);
        Task<EmployeeDTO?> GetEmployeeByCnicAsync(string cnic);
        Task<EmployeeDTO?> AddEmployeeAsync(EmployeeDTO employeeDto);
        Task<EmployeeDTO?> UpdateEmployeeAsync(EmployeePDTO employeePdto);
        Task<bool> DeleteEmployeeAsync(string cnic);
    }
}
