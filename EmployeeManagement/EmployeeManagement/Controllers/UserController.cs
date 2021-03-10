using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using EmployeeManagement.Custom;
using Microsoft.AspNetCore.Mvc;

namespace EmployeeManagement.Controllers
{
    public class UserController : Controller
    {
        [HttpGet]
        public IActionResult Index([ModelBinder(BinderType = typeof(CustomModelBinder))] User u)
        {
            return View();
        }
    }
}
