using EF_Core.Models;

namespace EF_Core.Repositories.Interfaces
{
    public interface ICustomerRepository
    {
        Task<IEnumerable<Customer>> GetAllCustomerAsync(int limit = 2, int offset = 0);
        Task<Customer?> GetCustomerByCnicAsync(string cnic);
        Task<Customer> AddCustomerAsync(Customer customer);
        Task<Customer> UpdateCustomerAsync(Customer customer);
        Task<bool> DeleteCustomerAsync(string cnic);
        Task<bool> Exist(string cnic);
    }
}
