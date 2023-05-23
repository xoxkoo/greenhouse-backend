using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
using System.Text.Json.Serialization;

namespace Domain.Entities;

public class Email
{
    [Key]
    public string EmailAddress { get; set; }
    [NotMapped]
    public string Title { get; set; }
    [NotMapped]
    public string Body { get; set; }
    
}