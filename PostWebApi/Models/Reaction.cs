using System.ComponentModel.DataAnnotations;

namespace PostWebApi.Models;

public class Reaction
{
    public int Id { get; set; }
    
    [Required(ErrorMessage = "UserId is required.")]
    public int UserId { get; set; }
    
    [Required(ErrorMessage = "Timeline is required.")]
    public DateTime Timeline { get; set; }
    
    // Foreign key to the Post entity
    [Required(ErrorMessage = "Postid is required.")]
    public int PostId { get; set; } // This is the foreign key for Post
    
    public Post? Post { get; set; } = null!;

}