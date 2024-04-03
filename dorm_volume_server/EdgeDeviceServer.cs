using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace dorm_volume_server;

internal class EdgeDeviceServer
{
    List<Client> connections=new();
    public static int port = 23456;
    ConnectionServer connectionServer;
    public EdgeDeviceServer()
    {
        connectionServer = new(port, ConnectHandler);
    }
    public void Shutdown()
    {
        connectionServer.close();
    }
    private void ConnectHandler(Connection connection)
    {
        Client client = new(connection);
        connections.Add(client);
        connection.onError += (e) =>
        {
            connections.Remove(client);
        };
    }
}
public class Client
{
    public Connection connection;
    public string? account;
    public Client(Connection connection)
    {
        this.connection = connection;
        connection.messageHander += onMessageReceived;
    }
    private void onMessageReceived(string msg)
    {

    }
}