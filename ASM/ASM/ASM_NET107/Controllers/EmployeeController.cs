using ASM_NET107.DAL;
using ASM_NET107.Models;
using Microsoft.AspNetCore.Mvc;

namespace ASM_NET107.Controllers
{
    public class EmployeeController : Controller
    {
        private readonly EmployeeDAL _employeeDAL;

        public EmployeeController(EmployeeDAL employeeDAL)
        {
            _employeeDAL = employeeDAL;
        }
        public IActionResult Index()
        {
            var employees = _employeeDAL.GetEmployees();
            return View(employees);
        }
        public IActionResult Create()
        {
            return View();
        }
        [HttpPost]
        public IActionResult Create(Employees employee)
        {
            if (ModelState.IsValid)
            {
                _employeeDAL.InsertEmployee(employee);
                return RedirectToAction("Employee");
            }
            return View(employee);
        }
        public IActionResult Delete()
        {
            var employees = _employeeDAL.GetActiveEmployees();
            return View(employees);
        }
        [HttpPost]
        public IActionResult Delete(string id)
        {
            _employeeDAL.DeleteEmployee(id);
            return RedirectToAction("Employee");
        }
        public IActionResult Edit(string id)
        {
            var employee = _employeeDAL.GetEmployeeById(id);
            if (employee == null)
            {
                return NotFound();
            }
            return View(employee);
        }
        [HttpPost]
        public IActionResult Edit(Employees employee)
        {
            if (ModelState.IsValid)
            {
                _employeeDAL.UpdateEmployee(employee);
                return RedirectToAction("Employee");
            }
            return View(employee);
        }
    }
}
