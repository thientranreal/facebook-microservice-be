namespace PostWebApi.Models;

public class Comment
{
    public int Id { get; set; }
    public int UserId { get; set; }
    public string Content { get; set; }
    public DateTime Timeline { get; set; }
    
    public Post Post { get; set; } = null!;
}