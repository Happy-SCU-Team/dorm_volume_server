namespace EventManger;

public class EventManager
{
    public event EventHandler<EventEvent> onEventRecord;
    public Level DisplayLevel { get; set; } = Level.Warn;
    private List<EventMessage> events = new();
    public EventManager Instance { get; private set; } = new();

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

    public void Add(string message)
    {
        Add(Level.Info,message);
    }
    public void Add(Level level,string message)
    {
        Add(new EventMessage { level=level,message=message});
    }
    public void Add(EventMessage eventMessage)
    {
        events.Add(eventMessage);
        onEventRecord.Invoke(this,new EventEvent { EventMessage=eventMessage});
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