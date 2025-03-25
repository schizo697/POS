using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace POS.Models
{
    public class Salary
    {
        [Key]
        public int SalaryId { get; set; }

        public int EmployeeId { get; set; }

        [Required]
        [Range(0, double.MaxValue)]
        public double GrandTotalHours { get; set; }

        [Column(TypeName = "decimal(18,0)")]
        public decimal? CashAdvance { get; set; }

        [Required]
        [Column(TypeName = "decimal(18,0)")]
        public decimal GrandTotalSalary { get; set; }

        [ForeignKey("EmployeeId")]
        public Employee? Employee { get; set; }
    }
}
