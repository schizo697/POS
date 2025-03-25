using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace POS.Models
{
    public class Dtr
    {
        [Key]
        public int DtrId { get; set; }

        // Foreign Key for Employee
        public int EmployeeId { get; set; }

        [DataType(DataType.Date)]
        [DisplayFormat(DataFormatString = "{0:yyyy-MM-dd}", ApplyFormatInEditMode = true)]
        public DateTime DtrDate { get; set; } = DateTime.Now.Date;

        [Required]
        [DataType(DataType.Time)]
        [DisplayFormat(DataFormatString = "{0:HH:mm:ss}", ApplyFormatInEditMode = true)]
        public DateTime TimeIn { get; set; }

        [DataType(DataType.Time)]
        [DisplayFormat(DataFormatString = "{0:HH:mm:ss}", ApplyFormatInEditMode = true)]
        public DateTime? TimeOut { get; set; }

        [NotMapped]
        public double Hours => TimeOut.HasValue ? (TimeOut.Value - TimeIn).TotalHours : 0;
        [NotMapped]
        public string FormattedHours => $"{(int)Hours}:{(int)((Hours - (int)Hours) * 60):D2}";

        [ForeignKey("EmployeeId")]
        public Employee? Employee { get; set; }
    }
}
