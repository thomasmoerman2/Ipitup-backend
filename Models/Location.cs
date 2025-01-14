using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

[Table("Locations")]
public class Location
{
    [Key]
    [DatabaseGenerated(DatabaseGeneratedOption.Identity)]
    public int LocationId { get; set; }

    [Required]
    [StringLength(100)]
    public string LocationName { get; set; } = string.Empty;
}