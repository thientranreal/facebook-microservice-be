using System.ComponentModel.DataAnnotations;

namespace ContactWebApi.Models;

public class Group
{
    public int GroupId { get; set; }

    [MaxLength(50)] public string GroupName { get; set; } = null!;
    
    public int CreatorId { get; set; }
    
    // Navigation properties
    public ICollection<Joining> Joinings { get; set; } = new List<Joining>();
    public ICollection<Message> Messages { get; set; } = new List<Message>();
}