using EF_Core.DTOs;
using EF_Core.DTOs.For_Patch;

namespace EF_Core.Services.Interfaces
{
    public interface ICustomerService
    {
        Task<IEnumerable<CustomerDTO?>> GetAllCustomerAsync(int limit = 2, int offset = 0);
        Task<CustomerDTO?> GetCustomerByCnicAsync(string cnic);
        Task<CustomerDTO?> AddCustomerAsync(CustomerDTO customerDto);
        Task<CustomerDTO?> UpdateCustomerAsync(CustomerPDTO customerDpto);
        Task<bool> DeleteCustomerAsync(string cnic);
    }
}
