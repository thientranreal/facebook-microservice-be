namespace PostWebApi.Models;

public class Story
{
    public int id { get; set; }
    public int userId { get; set; }
    public string image { get; set; }
    public DateTime timeline { get; set; }
}