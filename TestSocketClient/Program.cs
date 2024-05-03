using Conn;

//124.221.108.135
var conn = Connection.connect("124.221.108.135", 23456, messageHanlder);
conn.StartReceive();
void messageHanlder(string m)
{
    Console.WriteLine("data receive "+m);
}
int i = 1;

var t = new Thread(() => {
    //while (true)
    //{
    //    var msg = Console.ReadLine();
    //    conn.send($@"{{""message"":""{msg}"",""number"":{i++} }}");
    //}
    conn.send(@"{""type"":""login"",""account"":""114514"",""dorm"":1}");
    conn.send(@"{""type"":""request"",""content"":""schedule""}");
});
t.Start();

conn.receiveTask.Wait();

t.IsBackground = false;
