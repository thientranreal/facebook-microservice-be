namespace UserWebApi.Models;

public class ForgetPasswordRequest
{
    public string Email { get; set; }
    public DateOnly BirthDate { get; set; }
}
