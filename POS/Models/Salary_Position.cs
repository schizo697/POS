using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

public class PositionSalary
{
    [Key]
    public int Id { get; set; }

    [Required]
    public string Position { get; set; }

    public decimal Salary { get; set; }
}
