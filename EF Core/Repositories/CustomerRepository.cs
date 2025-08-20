using EF_Core.Models;
using EF_Core.Repositories.Interfaces;
using Microsoft.EntityFrameworkCore;
using YourNamespace.Data;

namespace EF_Core.Repositories
{
    public class CustomerRepository : ICustomerRepository
    {
        private AppDbContext _context;

        public CustomerRepository(AppDbContext context)
        {
            _context = context;
        }
        async Task<Customer> ICustomerRepository.AddCustomerAsync(Customer customer)
        {
            try
            {
                var cust = await _context.Customers.AddAsync(customer);
                await _context.SaveChangesAsync();
                return cust.Entity;
            }
            catch (Exception ex)
            {
                throw new Exception(ex.Message);
            }
        }

        async Task<bool> ICustomerRepository.DeleteCustomerAsync(string cnic)
        {
            try
            {
                var customer = await _context.Customers.FirstOrDefaultAsync(c => c.Cnic == cnic);
                if (customer == null)
                {
                    return (false);
                }
                _context.Customers.Remove(customer);
                await _context.SaveChangesAsync();
                return true;
            }
            catch (Exception ex)
            {
                throw new Exception(ex.Message);
            }
        }

        Task<bool> ICustomerRepository.Exist(string cnic)
        {
            try
            {
                return _context.Customers.AnyAsync(c => c.Cnic == cnic);
            }
            catch (Exception ex)
            {
                throw new Exception(ex.Message);
            }
        }

        async Task<IEnumerable<Customer>> ICustomerRepository.GetAllCustomerAsync(int limit, int offset)
        {
            try
            {
                return await _context.Customers.Take(limit).Skip(offset).ToListAsync();
            }
            catch (Exception ex)
            {
                throw new Exception(ex.Message);
            }
        }

        Task<Customer?> ICustomerRepository.GetCustomerByCnicAsync(string cnic)
        {
            try
            {
                return _context.Customers.FirstOrDefaultAsync(c => c.Cnic == cnic);
            }
            catch (Exception ex)
            {
                throw new Exception(ex.Message);
            }
        }

        async Task<Customer> ICustomerRepository.UpdateCustomerAsync(Customer customer)
        {
            try
            {
                _context.Customers.Update(customer);
                await _context.SaveChangesAsync();
                return customer;
            }
            catch (Exception ex)
            {
                throw new Exception(ex.Message);
            }
        }
    }
}
