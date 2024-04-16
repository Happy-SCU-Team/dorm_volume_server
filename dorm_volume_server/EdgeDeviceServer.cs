using EventManagerLib;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Text.Json;
using System.Threading.Tasks;
using Conn;

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
    public int dormID;
    private static string Serilize(Server2ClientJson.BaseJson json)
    {
        return JsonSerializer.Serialize(json,Server2ClientJson.BaseJson.jsonOptions);
    }
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
        string type = JsonSerializer.Deserialize<Client2ServerJson.BaseJson>(msg,Client2ServerJson.BaseJson.jsonOptions)!.type!;
        if (account == null ||dormID==0)
        {
            Client2ServerJson.LoginJson login = JsonSerializer.Deserialize<Client2ServerJson.LoginJson>(msg, Client2ServerJson.BaseJson.jsonOptions)!;
            account= login.account!;
            dormID = login.dorm;
        }
        else
        {
            switch (type)
            {
                case Protocol.Client2Server.volume:
                    Client2ServerJson.VolumeJson volume = 
                        JsonSerializer.Deserialize<Client2ServerJson.VolumeJson>
                        (msg, Client2ServerJson.BaseJson.jsonOptions)!;

                    break;
                case Protocol.Client2Server.request:
                    Client2ServerJson.RequestJson request = 
                        JsonSerializer.Deserialize<Client2ServerJson.RequestJson>
                        (msg, Client2ServerJson.BaseJson.jsonOptions)!;
                    break;
                default:
                    EventManager.Add(Level.Warn, "Unknown type " + type);
                    break;
            }
        }

    }
}