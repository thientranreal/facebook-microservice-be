namespace UserWebApi.Models;

public class Friend
{
    public int Id { get; set; }
    public bool IsFriend { get; set; }
    public DateTime TimeLine { get; set; }

    public int UserId1 { get; set; }  // Khóa ngoại tham chiếu đến User 1 (người kết bạn)
    public int UserId2 { get; set; }  // Khóa ngoại tham chiếu đến User 2 (người được kết bạn)

    // Thuộc tính điều hướng
    public User? User1 { get; set; }   // Người kết bạn (UserId1)
    public User? User2 { get; set; }   // Người được kết bạn (UserId2)
}
