namespace Ipitup_backend.Models;
[Table("Location")]
public class Location
{
    [Key]
    [DatabaseGenerated(DatabaseGeneratedOption.Identity)]
    public int LocationId { get; set; }
    public string LocationName { get; set; } = string.Empty;
}
