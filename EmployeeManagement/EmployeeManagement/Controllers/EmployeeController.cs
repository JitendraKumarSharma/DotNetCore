using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Threading.Tasks;
using EmployeeManagement.Models;
using EmployeeManagement.ViewModels;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Hosting;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Extensions.Logging;

namespace EmployeeManagement.Controllers
{
    //[Route("")]
    //[Route("jeet/[controller]")]
    //[Route("[controller]")]
    public class EmployeeController : Controller
    {
        private readonly IEmployeeRepository _employeeRepository;
        private readonly IHostingEnvironment _hostingEnvironment;
        private readonly ILogger _logger;

        public EmployeeController(IEmployeeRepository employeeRepository, IHostingEnvironment hostingEnvironment, ILogger<EmployeeController> logger)
        {
            _employeeRepository = employeeRepository;
            _hostingEnvironment = hostingEnvironment;
            _logger = logger;
        }

        public static int fun(int x)
        {
            byte y = (byte)x;
            return y;
        }
        //[Route("")]
        //[Route("~/")]
        //[Route("[action]")]
        [AllowAnonymous]
        public IActionResult Index()
        {
            var emplist = _employeeRepository.GetAllEmployee();
            return View(emplist);
        }
        //[Route("[action]/{id?}")]
        [AllowAnonymous]
        public IActionResult Details(int? id)
        {
            //throw new Exception("Error from details view");
            _logger.LogTrace("Trace Log");
            _logger.LogDebug("Debug Log");
            _logger.LogInformation("Information Log");
            _logger.LogWarning("Warning Log");
            _logger.LogError("Error Log");
            _logger.LogCritical("Critical Log");
            Employee employee = _employeeRepository.GetEmployeeById(id.Value);
            if(employee==null)
            {
                Response.StatusCode = 404;
                return View("EmployeeNotFound", id.Value);
            }
            var emp = employee;
            return View(emp);
        }

        //[Route("[action]")]
        public ViewResult Create()
        {
            return View();
        }

        [HttpPost]
        //[Route("[action]")]
        public IActionResult Create(EmployeeCreateViewModel model)
        {
            if (ModelState.IsValid)
            {
                string uniqueFileName = ProcessUploadedFiles(model);
                var newEmployee = new Employee
                {
                    Name = model.Name,
                    Email = model.Email,
                    Department = model.Department,
                    PhotoPath = uniqueFileName
                };
                _employeeRepository.Add(newEmployee);
                //var newEmployee = _employeeRepository.Add(model);
                return RedirectToAction("details", new { id = newEmployee.Id });
            }
            return View();
        }

        
        //[Route("[action]")]
        public ViewResult Edit(int id)
        {
            Employee employee = _employeeRepository.GetEmployeeById(id);
            EmployeeEditViewModel employeeEditViewModel = new EmployeeEditViewModel
            {
                Id = employee.Id,
                Name = employee.Name,
                Department = employee.Department,
                Email = employee.Email,
                ExistingPhotoPath = employee.PhotoPath
            };
            return View(employeeEditViewModel);
        }

        [HttpPost]
        //[Route("[action]")]
        public IActionResult Edit(EmployeeEditViewModel model)
        {
            if (ModelState.IsValid)
            {
                Employee employee = _employeeRepository.GetEmployeeById(model.Id);
                employee.Name = model.Name;
                employee.Email = model.Email;
                employee.Department = model.Department;
                if (model.Photos != null)
                {
                    if (model.ExistingPhotoPath != null)
                    {
                        var filePath = Path.Combine(_hostingEnvironment.WebRootPath, "images", model.ExistingPhotoPath);
                        System.IO.File.Delete(filePath);
                    }
                    employee.PhotoPath = ProcessUploadedFiles(model);
                }
                _employeeRepository.Update(employee);
                return RedirectToAction("index");
            }
            return View();
        }

        private string ProcessUploadedFiles(EmployeeCreateViewModel model)
        {
            string uniqueFileName = string.Empty;
            var uploadsFolder = Path.Combine(_hostingEnvironment.WebRootPath, "images");
            if (model.Photos != null && model.Photos.Count() > 1)
            {
                foreach (var photo in model.Photos)
                {
                    uniqueFileName = Guid.NewGuid().ToString() + "_" + photo.FileName;
                    string filePath = Path.Combine(uploadsFolder, uniqueFileName);
                    using (var fileStream =new FileStream(filePath,FileMode.Create))
                    {
                        photo.CopyTo(fileStream);
                    }
                    
                }
            }
            else if (model.Photos != null && model.Photos.Count() == 1)
            {
                uniqueFileName = Guid.NewGuid().ToString() + "_" + model.Photos[0].FileName;
                string filePath = Path.Combine(uploadsFolder, uniqueFileName);
                using (var fileStream = new FileStream(filePath, FileMode.Create))
                {
                    model.Photos[0].CopyTo(fileStream);
                }
               
            }

            return uniqueFileName;
        }
    }
}