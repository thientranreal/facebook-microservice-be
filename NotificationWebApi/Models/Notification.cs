namespace NotificationWebApi.Models;

public class Notification
{
    public int id { get; set; }
    public int user { get; set; }
    public int receiver { get; set; }
    public int post { get; set; }
    public string content { get; set; }
    public int is_read { get; set; }
    public DateTime timeline { get; set; }
}