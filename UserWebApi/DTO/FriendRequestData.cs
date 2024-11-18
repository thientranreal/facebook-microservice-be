namespace UserWebApi.Models;

public class FriendRequestData
{
    public int UserId1 { get; set; }
    public int UserId2 { get; set; }
    public bool IsFriend { get; set; }
    public string TimeLine { get; set; }
    public int RequestId { get; set; }
}