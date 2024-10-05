namespace PostWebApi.Models;

public class Post
{
    public int id { get; set; }
    public int userId { get; set; }
    public string content { get; set; }
    public string image { get; set; }
    public DateTime timeline { get; set; }
    
    public ICollection<Comment> Comments { get; set; } = new List<Comment>();
    public ICollection<Reaction> Reactions { get; set; } = new List<Reaction>();
}