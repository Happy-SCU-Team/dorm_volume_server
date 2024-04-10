using Conn;

var conn = Connection.connect("127.0.0.1",12345, messageHanlder);
conn.StartReceive();
void messageHanlder(string m)
{
    Console.WriteLine("data receive "+m);
}
int i = 1;
var t = new Thread(() => {
    while (true)
    {
        var msg = Console.ReadLine();
        conn.send($@"{{""message"":""{msg}"",""number"":{i++} }}");
    }
});
t.Start();

t.IsBackground = false;
