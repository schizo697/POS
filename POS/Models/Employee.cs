using Microsoft.EntityFrameworkCore;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace POS.Models
{
    [Index(nameof(Email), IsUnique = true)]
    public class Employee
    {
        [Key]
        public int EmployeeId { get; set; }

        [Required]
        [StringLength(100)]
        public string FirstName { get; set; }

        [Required]
        [StringLength(100)]
        public string LastName { get; set; }

        [NotMapped]
        public string FullName => $"{FirstName} {LastName}";

        [Required]
        [EmailAddress]
        public string Email { get; set; }

        public string Gender { get; set; }

        [Phone]
        public string PhoneNumber { get; set; }

        [Required]
        [StringLength(100)]
        public string employmentType { get; set; }

        public int? PositionId { get; set; }

        [ForeignKey("PositionId")]
        public Position? Position { get; set; }
    }
}
