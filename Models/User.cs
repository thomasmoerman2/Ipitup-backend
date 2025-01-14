using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

[Table("Users")]
public class User
{
    [Key]
    [DatabaseGenerated(DatabaseGeneratedOption.Identity)]
    public int UserId { get; set; }

    [Required]
    [StringLength(50)]
    public string UserFirstname { get; set; } = string.Empty;

    [Required]
    [StringLength(50)]
    public string UserLastname { get; set; } = string.Empty;

    [Required]
    [StringLength(100)]
    [EmailAddress]
    public string UserEmail { get; set; } = string.Empty;

    [Required]
    [StringLength(255)]
    public string UserPassword { get; set; } = string.Empty;

    [Required]
    public AccountStatus AccountStatus { get; set; }

    [Required]
    public int DailyStreak { get; set; }
}

public enum AccountStatus
{
    Public,
    Private
}