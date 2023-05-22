using System.ComponentModel.DataAnnotations;

namespace Domain.Entities;

public class User
{
    [Key]
    public string Email { get; set; }
    public string Password { get; set; }
    
}