using Microsoft.EntityFrameworkCore;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace EmployeeManagement.Models
{
    public static class ModelBuilderExtension
    {
        public static void Seed(this ModelBuilder modelBuilder)
        {
            modelBuilder.Entity<Employee>().HasData(
                new Employee
                {
                    Id = 1,
                    Name = "Raju",
                    Department = Dept.IT,
                    Email = "raju@gmail.com"
                },
                new Employee
                {
                    Id = 2,
                    Name = "Ajay",
                    Department = Dept.HR,
                    Email = "ajay@gmail.com"
                }
            );
        }
    }
}
