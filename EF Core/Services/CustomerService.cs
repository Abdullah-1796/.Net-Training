using EF_Core.DTOs;
using EF_Core.DTOs.For_Patch;
using EF_Core.Models;
using EF_Core.Repositories.Interfaces;
using EF_Core.Services.Interfaces;

namespace EF_Core.Services
{
    public class CustomerService : ICustomerService
    {
        private ICustomerRepository _customerRepository;

        public CustomerService(ICustomerRepository customerRepository)
        {
            _customerRepository = customerRepository;
        }

        async Task<CustomerDTO?> ICustomerService.AddCustomerAsync(CustomerDTO customerDto)
        {
            if(_customerRepository.Exist(customerDto.Cnic).Result)
            {
                return null;
            }
            Customer customer = new Customer
            {
                Cnic = customerDto.Cnic,
                Name = customerDto.Name,
                Phone = customerDto.Phone,
                Email = customerDto.Email
            };

            var cust = await _customerRepository.AddCustomerAsync(customer);
            return new CustomerDTO
            {
                Cnic = cust.Cnic,
                Name = cust.Name,
                Phone = cust.Phone,
                Email = cust.Email
            };
        }

        async Task<bool> ICustomerService.DeleteCustomerAsync(string cnic)
        {
            return await _customerRepository.DeleteCustomerAsync(cnic);
        }

        async Task<IEnumerable<CustomerDTO?>> ICustomerService.GetAllCustomerAsync(int limit, int offset)
        {
            var customers = await _customerRepository.GetAllCustomerAsync(limit, offset);
            return customers.Select(c => new CustomerDTO
            {
                Cnic = c.Cnic,
                Name = c.Name,
                Phone = c.Phone,
                Email = c.Email
            });
        }

        async Task<CustomerDTO?> ICustomerService.GetCustomerByCnicAsync(string cnic)
        {
            var customer = await _customerRepository.GetCustomerByCnicAsync(cnic);
            if (customer == null)
            {
                return null;
            }
            return new CustomerDTO
            {
                Cnic = customer.Cnic,
                Name = customer.Name,
                Phone = customer.Phone,
                Email = customer.Email
            };
        }

        async Task<CustomerDTO?> ICustomerService.UpdateCustomerAsync(CustomerPDTO customerDpto)
        {
            var existingCustomer = await _customerRepository.GetCustomerByCnicAsync(customerDpto.Cnic);
            if (existingCustomer == null)
            {
                return null;
            }
            existingCustomer.Name = customerDpto.Name ?? existingCustomer.Name;
            existingCustomer.Phone = customerDpto.Phone ?? existingCustomer.Phone;
            existingCustomer.Email = customerDpto.Email ?? existingCustomer.Email;
            var updatedCustomer = await _customerRepository.UpdateCustomerAsync(existingCustomer);

            return new CustomerDTO
            {
                Cnic = updatedCustomer.Cnic,
                Name = updatedCustomer.Name,
                Phone = updatedCustomer.Phone,
                Email = updatedCustomer.Email
            };
        }
    }
}
