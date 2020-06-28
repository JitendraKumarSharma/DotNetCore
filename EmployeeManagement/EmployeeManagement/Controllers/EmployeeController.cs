using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Threading.Tasks;
using EmployeeManagement.Models;
using EmployeeManagement.ViewModels;
using Microsoft.AspNetCore.Hosting;
using Microsoft.AspNetCore.Mvc;

namespace EmployeeManagement.Controllers
{
    [Route("jeet")]
    [Route("jeet/[controller]")]
    public class EmployeeController : Controller
    {
        private readonly IEmployeeRepository _employeeRepository;
        private readonly IHostingEnvironment _hostingEnvironment;

        public EmployeeController(IEmployeeRepository employeeRepository, IHostingEnvironment hostingEnvironment)
        {
            _employeeRepository = employeeRepository;
            _hostingEnvironment = hostingEnvironment;
        }
        [Route("")]
        [Route("~/")]
        [Route("[action]")]

        public IActionResult Index()
        {
            var emplist = _employeeRepository.GetAllEmployee();
            return View(emplist);
        }
        [Route("[action]/{id?}")]
        public IActionResult Details(int? id)
        {
            var emp = _employeeRepository.GetEmployeeById(id ?? 1);
            return View(emp);
        }

        [Route("[action]")]
        public ViewResult Create()
        {
            return View();
        }

        [HttpPost]
        [Route("[action]")]
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

        [Route("[action]")]
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
        [Route("[action]")]
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