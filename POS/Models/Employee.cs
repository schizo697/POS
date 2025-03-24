using System.ComponentModel.DataAnnotations.Schema;
using System.ComponentModel.DataAnnotations;

public class Employee
{
    [Key]
    public int Id { get; set; }

    [Required]
    public string FirstName { get; set; }

    [Required]
    public string LastName { get; set; }

    [Required]
    public string Gender { get; set; }

    [Required, EmailAddress]
    public string Email { get; set; }

    [Required]
    public string Role { get; set; }

    [ForeignKey("PositionSalary")]
    public int PositionId { get; set; }

    public PositionSalary PositionSalary { get; set; }
}
