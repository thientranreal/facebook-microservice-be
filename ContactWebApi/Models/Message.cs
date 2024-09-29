using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace ContactWebApi.Models;

public class Message
{
    public int Id { get; set; }
    
    public int Sender { get; set; }
    
    public int? Receiver { get; set; }
    
    [ForeignKey("Group")]
    public int? GroupId { get; set; }
    
    public DateTime CreatedAt { get; set; }

    [MaxLength(100)] public string Content { get; set; } = null!;

    // Navigation properties
    public Group Group { get; set; } = null!;
}