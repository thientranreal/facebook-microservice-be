using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace PostWebApi.Models;

public class Post
{
    public int id { get; set; }
    
    [Required(ErrorMessage = "UserId is required.")]
    public int userId { get; set; }
    
    public string content { get; set; } = null;

    public string? image { get; set; } = null;
    
    [Required(ErrorMessage = "Timeline is required.")]
    public DateTime timeline { get; set; }
    
    public ICollection<Comment> Comments { get; set; } = new List<Comment>();
    public ICollection<Reaction> Reactions { get; set; } = new List<Reaction>();
    
    [NotMapped]
    public bool likedByCurrentUser  { get; set; } = false;
}