using EF_Core.DTOs;
using EF_Core.Models;
using EF_Core.Repositories.Interfaces;
using EF_Core.Services.Interfaces;
using EF_Core.DTOs.For_Patch;
using Microsoft.AspNetCore.Http.HttpResults;
using AutoMapper;

namespace EF_Core.Services
{
    public class EmployeeService : IEmployeeService
    {
        private IEmployeeRepository employeeRepository;
        private readonly IMapper _mapper;

        public EmployeeService(IEmployeeRepository employeeRepository, IMapper mapper)
        {
            this.employeeRepository = employeeRepository;
            _mapper = mapper;
        }
        async Task<EmployeeDTO?> IEmployeeService.AddEmployeeAsync(EmployeeDTO employeeDto)
        {
            if(employeeRepository.Exist(employeeDto.Cnic).Result)
            {
                return null;
            }
            //Employee employee = new Employee
            //{
            //    Cnic = employeeDto.Cnic,
            //    Name = employeeDto.Name,
            //    Phone = employeeDto.Phone,
            //    Email = employeeDto.Email,
            //    Role = employeeDto.Role
            //};
            Employee employee = _mapper.Map<Employee>(employeeDto);
            var emp = await employeeRepository.AddEmployeeAsync(employee);
            //return new EmployeeDTO
            //{
            //    Cnic = emp.Cnic,
            //    Name = emp.Name,
            //    Phone = emp.Phone,
            //    Email = emp.Email,
            //    Role = emp.Role
            //};

            var employeeDto1 = _mapper.Map<EmployeeDTO>(employee);
            return employeeDto1;
        }

        async Task<bool> IEmployeeService.DeleteEmployeeAsync(string cnic)
        {
            return await employeeRepository.DeleteEmployeeAsync(cnic);
        }

        async Task<IEnumerable<EmployeeDTO?>> IEmployeeService.GetAllEmployeesAsync(int limit = 2, int offset = 0)
        {
            var employees = await employeeRepository.GetAllEmployeesAsync(limit, offset);
            //var employeeDtos = employees.Select(emp => new EmployeeDTO
            //{
            //    Cnic = emp.Cnic,
            //    Name = emp.Name,
            //    Phone = emp.Phone,
            //    Email = emp.Email,
            //    Role = emp.Role
            //});
            var employeeDtos = _mapper.Map<IEnumerable<EmployeeDTO>>(employees);
            return employeeDtos;
        }

        async Task<EmployeeDTO?> IEmployeeService.GetEmployeeByCnicAsync(string cnic)
        {
            var employee = await employeeRepository.GetEmployeeByCnicAsync(cnic);
            if (employee == null)
            {
                return null; // Employee not found
            }
            var employeeDto = _mapper.Map<EmployeeDTO>(employee);
            return employeeDto;
        }

        async Task<EmployeeDTO?> IEmployeeService.UpdateEmployeeAsync(EmployeePDTO employeePdto)
        {
            var existingEmployee = await employeeRepository.GetEmployeeByCnicAsync(employeePdto.Cnic);
            if (existingEmployee == null) {
                return null; // Employee not found
            }
            existingEmployee.Name = employeePdto.Name ?? existingEmployee.Name;
            existingEmployee.Phone = employeePdto.Phone ?? existingEmployee.Phone;
            existingEmployee.Email = employeePdto.Email ?? existingEmployee.Email;
            existingEmployee.Role = employeePdto.Role ?? existingEmployee.Role;

            var updatedEmployee = await employeeRepository.UpdateEmployeeAsync(existingEmployee);
            var employeeDto = _mapper.Map<EmployeeDTO>(updatedEmployee);
            return employeeDto;
        }
    }
}
