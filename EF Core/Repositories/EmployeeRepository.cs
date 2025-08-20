using EF_Core.DTOs;
using EF_Core.Models;
using EF_Core.Repositories.Interfaces;
using Microsoft.EntityFrameworkCore;
using System.Collections.Generic;
using YourNamespace.Data;

namespace EF_Core.Repositories
{
    public class EmployeeRepository : IEmployeeRepository
    {
        private AppDbContext context;

        public EmployeeRepository(AppDbContext context)
        {
            this.context = context;
        }

        async Task<Employee> IEmployeeRepository.AddEmployeeAsync(Employee employee)
        {
            try
            {
                var emp = await context.Employees.AddAsync(employee);
                await context.SaveChangesAsync();
                return emp.Entity;
            }
            catch (DbUpdateException ex)
            {
                throw new Exception("An error occurred while adding the employee.", ex);
            }
        }

        async Task<bool> IEmployeeRepository.DeleteEmployeeAsync(string cnic)
        {
            try
            {
                var employee = await context.Employees.FirstOrDefaultAsync(e => e.Cnic == cnic);
                if (employee == null)
                {
                    return false; // Employee not found
                }
                context.Employees.Remove(employee);
                await context.SaveChangesAsync();
                return true; // Employee deleted successfully
            }
            catch (Exception ex)
            {
                throw new Exception("An error occurred while deleting the employee.", ex);
            }
        }

        async Task<IEnumerable<Employee>> IEmployeeRepository.GetAllEmployeesAsync(int limit = 2, int offset = 0)
        {
            try
            {
                var employees = await context.Employees.Skip(offset).Take(limit).ToListAsync();
                return employees;
            }
            catch (Exception ex)
            {
                throw new Exception("An error occurred while retrieving employees.", ex);
            }
        }

        async Task<Employee?> IEmployeeRepository.GetEmployeeByCnicAsync(string cnic)
        {
            try
            {
                var emp = await context.Employees.Where(e => e.Cnic == cnic).FirstOrDefaultAsync();
                if (emp == null)
                {
                    return null; // Employee not found
                }
                Console.WriteLine(emp.Name);
                return emp;
            }
            catch (Exception ex)
            {
                throw new Exception(ex.Message);
            }
        }

        async Task<Employee> IEmployeeRepository.UpdateEmployeeAsync(Employee employee)
        {
            try
            {
                context.Employees.Update(employee);
                await context.SaveChangesAsync();
                return employee; // Return the updated employee
            }
            catch (Exception ex)
            {
                throw new Exception("An unexpected error occurred while updating the employee.", ex);
            }
        }

        async Task<bool> IEmployeeRepository.Exist(string cnic)
        {
            try
            {
                var emp = await context.Employees.FirstOrDefaultAsync(e => e.Cnic == cnic);
                if (emp == null)
                {
                    return false; // Employee does not exist
                }
                return true; // Employee exists
            }
            catch (Exception ex)
            {
                throw new Exception($"An error occurred while checking if the employee with CNIC {cnic} exists.", ex);
            }
        }
    }
}
