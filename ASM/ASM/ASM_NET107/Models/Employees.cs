using static System.Runtime.InteropServices.JavaScript.JSType;

namespace ASM_NET107.Models
{
    public class Employees
    {
        public string EmployeeID { get; set; }
        public string FullName { get; set; }
        public string Email { get; set; }
        public string Phone { get; set; }
        public string Username { get; set; }
        public string PasswordHash { get; set; }
        public string Role { get; set; }
        public DateOnly CreatedDate { get; set; }
        public bool IsActive { get; set; }
    }
}
