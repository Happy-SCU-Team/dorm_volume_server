using EventManagerLib;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Text.Json;
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
        connection.messageHander += e=> {
            try
            {
                onMessageReceived(e);
            }catch(Exception ex)
            {
                EventManager.Add(Level.Warn,"server can not handle client message "+ex.ToString());
            }
        };
    }
    private void onMessageReceived(string msg)
    {
        string type = JsonSerializer.Deserialize<BaseJson>(msg,BaseJson.jsonOptions)!.type!;
        if (account == null)
        {        
            LoginJson login = JsonSerializer.Deserialize<LoginJson>(msg)!;
            account= login.account!;
        }
        else
        {
            switch (type)
            {
                case Protocol.Client2Server.volume:
                    VolumeJson volume = JsonSerializer.Deserialize<VolumeJson>(msg)!;
                    break;
                case Protocol.Client2Server.request:
                    RequestJson request = JsonSerializer.Deserialize<RequestJson>(msg)!;
                    break;
                default:
                    EventManager.Add(Level.Info, "Unknown type " + type);
                    break;
            }
        }

    }
}