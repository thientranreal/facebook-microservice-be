using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace PostWebApi.Models;

public class Post
{
    public int id { get; set; }
    
    [Required(ErrorMessage = "UserId is required.")]
    public int userId { get; set; }
    
    [Required(ErrorMessage = "Content is required.hehe")]
    [StringLength(1000, ErrorMessage = "Content cannot exceed 1000 characters.")]
    public string content { get; set; }
    
    public string image { get; set; }
    
    [Required(ErrorMessage = "Timeline is required.")]
    public DateTime timeline { get; set; }
    
    public ICollection<Comment> Comments { get; set; } = new List<Comment>();
    public ICollection<Reaction> Reactions { get; set; } = new List<Reaction>();
    
    [NotMapped]
    public bool likedByCurrentUser  { get; set; } = false;
}