namespace UserWebApi.Models;

public class Request
{
    public int Id { get; set; }
    public int Sender { get; set; }
    public int Receiver { get; set; }
    public DateTime Timeline { get; set; }
}