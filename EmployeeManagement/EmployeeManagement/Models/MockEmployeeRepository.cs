using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace EmployeeManagement.Models
{
    public class MockEmployeeRepository : IEmployeeRepository
    {
        public List<Employee> _empList;
        public MockEmployeeRepository()
        {
            _empList = new List<Employee>()
            {
                new Employee() { Id = 1, Name = "Raj", Email = "raj@gmail.com", Department=Dept.IT },
                new Employee() { Id = 2, Name = "Jeet", Email = "jeet@gmail.com", Department=Dept.IT},
                new Employee() { Id = 3, Name = "Shweta", Email = "sh@gmail.com", Department=Dept.HR }
            };
        }

        public Employee Add(Employee employee)
        {
            employee.Id = _empList.Max(e => e.Id) + 1;
            _empList.Add(employee);
            return employee;
        }

        public Employee Delete(int id)
        {
            var employee=_empList.FirstOrDefault(e => e.Id == id);
            if(employee!=null)
            {
                _empList.Remove(employee);
            }
            return employee;
        }

        public IEnumerable<Employee> GetAllEmployee()
        {
            return _empList;
        }

        public Employee GetEmployeeById(int id)
        {
            return _empList.Where(e => e.Id == id).FirstOrDefault();
        }

        public Employee Update(Employee employeeChanges)
        {
            var employee = _empList.FirstOrDefault(e => e.Id == employeeChanges.Id);
            if (employee != null)
            {
                employee.Name = employeeChanges.Name;
                employee.Email = employeeChanges.Email;
                employee.Department = employeeChanges.Department;
            }
            return employee;
        }
    }
}
