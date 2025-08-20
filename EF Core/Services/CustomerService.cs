using AutoMapper;
using EF_Core.DTOs;
using EF_Core.DTOs.For_Patch;
using EF_Core.Models;
using EF_Core.Repositories.Interfaces;
using EF_Core.Services.Interfaces;

namespace EF_Core.Services
{
    public class CustomerService : ICustomerService
    {
        private readonly ICustomerRepository _customerRepository;

        private readonly IMapper _mapper;

        public CustomerService(ICustomerRepository customerRepository, IMapper mapper)
        {
            _customerRepository = customerRepository;
            _mapper = mapper;
        }

        async Task<CustomerDTO?> ICustomerService.AddCustomerAsync(CustomerDTO customerDto)
        {
            if(_customerRepository.Exist(customerDto.Cnic).Result)
            {
                return null;
            }
            Customer customer = _mapper.Map<Customer>(customerDto);

            var cust = await _customerRepository.AddCustomerAsync(customer);
            CustomerDTO customerDTO = _mapper.Map<CustomerDTO>(cust);
            return customerDTO;
        }

        async Task<bool> ICustomerService.DeleteCustomerAsync(string cnic)
        {
            return await _customerRepository.DeleteCustomerAsync(cnic);
        }

        async Task<IEnumerable<CustomerDTO?>> ICustomerService.GetAllCustomerAsync(int limit, int offset)
        {
            var customers = await _customerRepository.GetAllCustomerAsync(limit, offset);
            var customerDtos = _mapper.Map<IEnumerable<CustomerDTO>>(customers);
            return customerDtos;
        }

        async Task<CustomerDTO?> ICustomerService.GetCustomerByCnicAsync(string cnic)
        {
            var customer = await _customerRepository.GetCustomerByCnicAsync(cnic);
            if (customer == null)
            {
                return null;
            }
            CustomerDTO customerDto = _mapper.Map<CustomerDTO>(customer);
            return customerDto;
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


            CustomerDTO customerDto = _mapper.Map<CustomerDTO>(updatedCustomer);
            return customerDto;
        }
    }
}
