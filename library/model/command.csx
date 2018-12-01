public class Command
{
    public string id { get; set; }
    public Int32 sendTime { get; set; }
    public Int32 receptionTime { get; set; }
    public Int32 servedTime { get; set; }
    public Int32 durationToServe { get; set; }
    public string table { get; set; }
    public string drink { get; set; }
    public string status { get; set; }
}

public class CommandToReturn
{
    public string id { get; set; }
    public string drink { get; set; }
    public string sendTime { get; set; }
}
