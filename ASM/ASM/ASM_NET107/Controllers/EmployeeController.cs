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
            if (HttpContext.Session.GetString("UserRole") != "Manager")
                return RedirectToAction("Login", "Account");

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
            ModelState.Remove("EmployeeID");
            employee.CreatedDate = DateOnly.FromDateTime(DateTime.Now);
            if (ModelState.IsValid)
            {
                try
                {
                    _employeeDAL.InsertEmployee(employee);

                    return RedirectToAction("Index");
                }
                catch (Exception ex)
                {
                    ModelState.AddModelError("", "Lỗi lưu dữ liệu: " + ex.Message);
                }
            }

            return View(employee);
        }

        public IActionResult Delete(string id)
        {
            var employee = _employeeDAL.GetEmployeeById(id);
            if (employee == null) return NotFound();
            return View(employee);
        }

        [HttpPost]
        public IActionResult DeleteConfirmed(string id)
        {
            _employeeDAL.DeleteEmployee(id);
            return RedirectToAction("Index");
        }
        [HttpGet]
        public IActionResult Edit(string id)
        {
            if (string.IsNullOrEmpty(id))
            {
                return NotFound();
            }
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
                var oldData = _employeeDAL.GetEmployeeById(employee.EmployeeID);
                if (oldData != null)
                {
                    employee.CreatedDate = oldData.CreatedDate;
                }

                _employeeDAL.UpdateEmployee(employee);
                return RedirectToAction("Index");
            }
            return View(employee);
        }
        // 2. Action GET cho chức năng Xem chi tiết
        public IActionResult Details(string id)
        {
            if (string.IsNullOrEmpty(id))
            {
                return NotFound();
            }
            var employee = _employeeDAL.GetEmployeeById(id);
            if (employee == null)
            {
                return NotFound();
            }
            return View(employee);
        }

    }
}
