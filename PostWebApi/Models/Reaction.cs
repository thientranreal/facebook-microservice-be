namespace PostWebApi.Models;

public class Reaction
{
    public int Id { get; set; }
    public int UserId { get; set; }
    public DateTime Timeline { get; set; }
    
    public Post Post { get; set; } = null!;

}