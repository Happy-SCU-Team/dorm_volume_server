using System.Net.Sockets;
using System.Net;
using System.Text;

namespace Conn;

public class Connection
{
    public Exception? Exception { get; private set; }
    public readonly string EOF = "\n";
    public event onError? onError;
    public static int BufferSize { set; get; } = 4096;
    private StreamReader reader;
    public static Connection connect(string ip, int port, MessageHander hander)
    {
        TcpClient client = new TcpClient();
        client.Connect(IPAddress.Parse(ip), port);
        Connection conn = new Connection(client, hander);
        return conn;
    }
    public delegate void MessageHander(string msg);

    public readonly TcpClient TCPclient;
    //public CancellationTokenSource cts {  get; private set; }
    private NetworkStream stream;
    public MessageHander? messageHander { get; set; }
    public Connection(TcpClient client, MessageHander hander) : this(client)
    {
        this.messageHander = hander;
    }

    public Connection(TcpClient client)
    {
        //cts = new();
        this.TCPclient = client;
        stream = client.GetStream();
        
    }
    public void StartReceive()
    {
        reader = new(stream);
        receiveTask = receive();
        
    }
    public void close()
    {
        //cts.Cancel();
        reader.Close();
        stream.Close();
        TCPclient.Close();

    }

    public Task<int> receiveTask { get; private set; }
    public virtual async void send(string msg)
    {

        Console.WriteLine("Sending: " + msg);

        byte[] responseData = Encoding.UTF8.GetBytes(msg + EOF);
        await stream.WriteAsync(responseData, 0, responseData.Length);
        await stream.FlushAsync();
    }
    private async Task<int> receive()
    {
        try
        {
            //await ReceiveObsolete();
            await ReceiveCore();
        }
        catch (TaskCanceledException)
        {
            return 0;
        }
        catch (Exception ex)
        {
            Exception = ex;
            onError?.Invoke(ex);
            return -1;
        }
        return 0;
        [Obsolete]
        async Task ReceiveObsolete()
        {
            while (true)
            {
                byte[] buffer = new byte[BufferSize];
                int bytesRead;

                while ((bytesRead = await stream.ReadAsync(buffer, 0, buffer.Length)) > 0)
                {
                    string dataReceived = Encoding.UTF8.GetString(buffer, 0, bytesRead);
                    Console.WriteLine($"Received: {dataReceived}");
                    var m = dataReceived.Split(EOF);
                    for (int i = 0; i < m.Length - 1; i++)
                    {
                        messageHander?.Invoke(m[i]);
                    }

                }
            }
        }
        async Task ReceiveCore()
        {
            while (true)
            {
                string c;
                while (!string.IsNullOrEmpty(c = await reader.ReadLineAsync()))
                {
                    string dataReceived = c;
                    Console.WriteLine($"Received: {dataReceived}");
                    messageHander?.Invoke(dataReceived);
                    

                }
            }
        }
    }
}


public delegate void ConnectionHandler(Connection connection);
public delegate void onError(Exception e);
public class ConnectionServer
{
    ConnectionHandler handler;
    TcpListener server;
    public Thread thread { get; private set; }

    TcpListener? server_v6;
    public Thread? thread_v6 { get; private set; }

    public event onError OnError;


    /// <summary>
    /// Build IPv4 Server
    /// </summary>
    /// <param name="port"></param>
    /// <param name="handler">How to deal with the connection</param>
    public ConnectionServer(int port, ConnectionHandler handler) : this(port, handler, isEnableIPv6: true) { }

    public ConnectionServer(int port, ConnectionHandler handler, bool isEnableIPv6)

    {
        this.handler = handler;
        if (isEnableIPv6)
        {
            Console.WriteLine("IPv6 server has been enabled");
            server_v6 = new TcpListener(IPAddress.IPv6Any, port);
            server_v6.Start();
            thread_v6 = new(() => Start(server_v6));
            thread_v6.Start();
        }

        server = new TcpListener(IPAddress.Any, port);
        server.Start();
        thread = new(() => Start(server));
        thread.Start();
    }
    public void Wait()
    {
        thread.Join();
        thread_v6?.Join();
    }


    public void close()
    {
        server_v6?.Stop();
        server.Stop();
        thread_v6?.Interrupt();
        thread.Interrupt();

    }
    private void Start(TcpListener _server)
    {
        Console.WriteLine($"Network Server {_server.LocalEndpoint.AddressFamily} Start");
        while (true)
        {
            try
            {
                TcpClient client = _server.AcceptTcpClient();
                handler.Invoke(new Connection(client));
            }
            catch (ThreadInterruptedException)
            {
                //throw e;
                break;
            }
            catch (InvalidOperationException)
            {
                break;
            }
            catch (Exception e)
            {
                OnError?.Invoke(e);
            }
        }
        Console.WriteLine("Network Server Shutdown");
    }
}