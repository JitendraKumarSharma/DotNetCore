using Microsoft.AspNetCore.Mvc;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace EmployeeManagement.Custom
{
    [ModelBinder(BinderType = typeof(CustomModelBinder))]
    public class User
    {
        public int Id { get; set; }
        public string Name { get; set; }
    }
}
