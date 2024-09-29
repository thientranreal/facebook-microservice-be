namespace ContactWebApi.Models;

public class Joining
{
    public int Id { get; set; }
    
    public int User { get; set; }
    
    public DateTime JoiningDate { get; set; }

    // Navigation property
    public Group Group { get; set; } = null!;
}