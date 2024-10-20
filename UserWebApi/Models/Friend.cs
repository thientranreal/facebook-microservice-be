namespace UserWebApi.Models;

public class Friend
{
    public int Id { get; set; }
    public int UserId1 { get; set; }
    public int UserId2 { get; set; }
    public bool IsFriend { get; set; }
    public DateTime TimeLine { get; set; }
}
