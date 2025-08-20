using EF_Core.Models;

namespace EF_Core.Repositories.Interfaces
{
    public interface IEmployeeRepository
    {
        Task<IEnumerable<Employee>> GetAllEmployeesAsync(int limit = 2, int offset = 0);
        Task<Employee?>  GetEmployeeByCnicAsync(string cnic);
        Task<Employee> AddEmployeeAsync(Employee employee);
        Task<Employee> UpdateEmployeeAsync(Employee employee);
        Task<bool> DeleteEmployeeAsync(string cnic);
        Task<bool> Exist(string cnic);
    }
}
