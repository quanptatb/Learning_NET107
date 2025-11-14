using System.ComponentModel.DataAnnotations;

namespace Bai3.Models
{
    public class Employee
    {
        [Display(Name = "Serial No")]
        public byte EmployeeId { get; set; }

        [Display(Name = "Name")]
        public string EmployeeName { get; set; }

        public string Address { get; set; }
        public string Phone { get; set; }
    }
}
