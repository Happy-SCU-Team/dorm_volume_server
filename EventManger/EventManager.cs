namespace EventManagerLib;

public class EventManager
{
    public event EventHandler<EventEvent> onEventRecord;
    public static Level DisplayLevel { get; set; } = Level.Warn;
    private List<EventMessage> events = new();
    public static EventManager Instance { get; private set; } = new();

    public IEnumerable<EventMessage> Events => events;

    private EventManager()
    {
        onEventRecord += EventManager_onEventRecord;
    }

    private void EventManager_onEventRecord(object? sender, EventEvent e)
    {
        if (e.EventMessage.level >= DisplayLevel)
        {
            Console.WriteLine($"[{e.EventMessage.level}]:{e.EventMessage.message}");
        }
    }

    public static void  Add(string message)
    {
        Add(Level.Info,message);
    }
    public static void Add(Level level,string message)
    {
        Add(new EventMessage { level=level,message=message});
    }
    public static void Add(EventMessage eventMessage)
    {
        Instance.events.Add(eventMessage);
        Instance.onEventRecord.Invoke(Instance,new EventEvent { EventMessage=eventMessage});
    }
}
public enum Level
{
    Info,Warn,Error,
}
public struct EventMessage
{
    public Level level;
    public string message;
}
public class EventEvent : EventArgs
{
    public EventMessage EventMessage;
}