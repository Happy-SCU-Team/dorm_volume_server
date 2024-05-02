using EventManagerLib;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Text.Json;
using System.Threading.Tasks;
using Conn;
using DataProvider;

namespace dorm_volume_server;

internal class EdgeDeviceServer
{
    List<Client> connections=new();
    public static int port = 23456;
    ConnectionServer connectionServer;
    DataProvider.DataProvider dataProvider;
    public EdgeDeviceServer(DataProvider.DataProvider dataProvider)
    {
        connectionServer = new(port, ConnectHandler);
        this.dataProvider = dataProvider;
    }
    public bool isOnline(string account)
    {
        foreach (Client client in connections)
        {
            if (client.account == account)
            {
                return true;
            }
        }
        return false;
    }
    public void Shutdown()
    {
        connectionServer.close();
    }
    private void ConnectHandler(Connection connection)
    {
        Client client = new(connection,dataProvider);
        connections.Add(client);
        connection.onError += (e) =>
        {
            connections.Remove(client);
        };
    }
    public void UpdateAccount(string account,string new_account)
    {
        foreach(var i in connections)
        {
            if(i.account == account)
            {
                i.SendAccountUpdateJson(account);
                return;
            }
        }
    }
    public void Update(string account)
    {
        foreach (var i in connections)
        {
            if (i.account == account)
            {
                i.Update();
                return;
            }
        }
    }
}
public class Client
{
    public Connection connection;
    public string? account;
    public int dormID;
    DataProvider.DataProvider dataProvider;
    private void SendObject(Server2ClientJson.BaseJson json)
    {
        connection.send(Serilize(json));
    }
    private static string Serilize(Server2ClientJson.BaseJson json)
    {
        return JsonSerializer.Serialize(json,Server2ClientJson.BaseJson.jsonOptions);
    }
    public Client(Connection connection, DataProvider.DataProvider dataProvider)
    {
        this.connection = connection;
        connection.messageHander += e =>
        {
            try
            {
                onMessageReceived(e);
            }
            catch (Exception ex)
            {
                EventManager.Add(Level.Warn, "server can not handle client message " + ex.ToString());
            }
        };
        connection.StartReceive();
        this.dataProvider = dataProvider;
    }
    private void onMessageReceived(string msg)
    {
        string type = JsonSerializer.Deserialize<Client2ServerJson.BaseJson>(msg,Client2ServerJson.BaseJson.jsonOptions)!.type!;
        if (account == null ||dormID==0)
        {
            Client2ServerJson.LoginJson login = JsonSerializer.Deserialize<Client2ServerJson.LoginJson>(msg, Client2ServerJson.BaseJson.jsonOptions)!;
            account= login.account;
            dormID = login.dorm;
            if (account != null &&dataProvider.CheckAccountExistance(account) == false)
            {
                dataProvider.register(account);
            }
        }
        else
        {
            switch (type)
            {
                case Protocol.Client2Server.volume:
                    Client2ServerJson.VolumeJson volume = 
                        JsonSerializer.Deserialize<Client2ServerJson.VolumeJson>
                        (msg, Client2ServerJson.BaseJson.jsonOptions)!;
                    handleVolume(volume);
                    break;
                case Protocol.Client2Server.request:
                    Client2ServerJson.RequestJson request = 
                        JsonSerializer.Deserialize<Client2ServerJson.RequestJson>
                        (msg, Client2ServerJson.BaseJson.jsonOptions)!;
                    handleRequest(request);
                    break;
                default:
                    EventManager.Add(Level.Warn, "Unknown type " + type);
                    break;
            }
        }
    }
    void handleVolume(Client2ServerJson.VolumeJson volume)
    {
        VolumeType volumeType = 
            volume.type == VolumeType.alert.ToString() ? VolumeType.alert : VolumeType.info;
        var volumeInfo = new VolumeInfo
        {
            time = new DateTime(volume.time),
            volumeType = volumeType,
            volume = volume.volume,
            dormId = volume.dorm,
        };
        dataProvider.RecordVolumeInfo(account!, volumeInfo);
        if(volumeType== VolumeType.alert)
        {
            QQ_Alert.alert(account!, volumeInfo);
        }

    }
    void handleRequest(Client2ServerJson.RequestJson request)
    {
        switch(request.content)
        {
            case Protocol.Client2Server.Request.schedule:
                SendObject(new Server2ClientJson.ScheduleUpdateJson {
                   schedule=dataProvider.GetScheduleSegments(account!)
                });
                break;
        }
    }
    public void SendAccountUpdateJson(string new_account)
    {
        this.account = new_account;
        SendObject(new Server2ClientJson.AccountUpdateJson
        {
            account=new_account,
        });
    }
    public void Update()
    {
        SendObject(new Server2ClientJson.IntervalUpdateJson { 
            interval=dataProvider.getInterval(account!),
        });
        SendObject(new Server2ClientJson.ScheduleUpdateJson
        {
            schedule=dataProvider.GetScheduleSegments(account!)
        });
    }
}