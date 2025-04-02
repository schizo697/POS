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

        [NotMapped]
        public string FormattedGrandTotalHours => $"{(int)GrandTotalHours}:{(int)((GrandTotalHours - (int)GrandTotalHours) * 60):D2}";

        [Column(TypeName = "decimal(18,2)")]
        public decimal? CashAdvance { get; set; }

        [Required]
        [Column(TypeName = "decimal(18,2)")]
        public decimal GrandTotalSalary { get; set; }

        [Required]
        public string Status { get; set; } = "Unpaid";

        [ForeignKey("EmployeeId")]
        public Employee? Employee { get; set; }
    }
}
