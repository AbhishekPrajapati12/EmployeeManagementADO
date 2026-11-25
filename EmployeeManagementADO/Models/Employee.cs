using System.ComponentModel.DataAnnotations;

namespace EmployeeManagementADO.Models
{
    public class Employee
    {
        public int EmployeeId { get; set; }

        [Required]
        [StringLength(100, MinimumLength = 3)]
        [RegularExpression(@"^[A-Za-z ]+$")]
        public string Name { get; set; } = "";

        [Required]
        public string Gender { get; set; } = "";

        [Range(1, int.MaxValue)]
        public int DepartmentId { get; set; }

        [Required]
        public string State { get; set; } = "";

        [Required]
        public string City { get; set; } = "";

        [Range(typeof(decimal), "1", "10000000")]
        public decimal Salary { get; set; }

        public string DepartmentName { get; set; } = "";
    }
}
