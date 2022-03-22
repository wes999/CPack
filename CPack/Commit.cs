namespace CPack;

public class Commit
{
    public DateTime Time { get; set; }
    public string Message { get; set; }

    public Commit(string message, DateTime time)
    {
        Message = message;
        Time = time;
    }
}