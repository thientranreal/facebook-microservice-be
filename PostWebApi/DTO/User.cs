namespace PostWebApi.DTO;

public class RootObject
{
    public string Id { get; set; }
    public List<User> Values { get; set; }
}

public class User
{
    public int Id { get; set; }
    public string Name { get; set; }
    public string Birth { get; set; }
    public string Avt { get; set; }
    public string Phone { get; set; }
    public string Email { get; set; }
    public string Gender { get; set; }
    public string Desc { get; set; }
    public int IsOnline { get; set; }
    public string LastActive { get; set; }
    public string Password { get; set; }
    public string Address { get; set; }
    public string Social { get; set; }
    public string Education { get; set; }
    public string Relationship { get; set; }
    public string TimeJoin { get; set; }
}
