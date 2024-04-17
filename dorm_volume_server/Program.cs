//launch web server

using DataProvider;
using dorm_volume_server;
using EventManagerLib;

bool isDebug = true;

var dataProvider=new SimpleDataProvider(isDebug: isDebug);
if (isDebug)
{
    EventManager.DisplayLevel = Level.Info;
}

var edgeDeviceServer=new EdgeDeviceServer(dataProvider);

Console.WriteLine("---Accounts---");
foreach(var i in dataProvider.GetAccounts())
{
    Console.WriteLine(i);
}
Console.WriteLine("--------------");

//-----WEB-----
web_server.Server.onSettingChanged += Server_onSettingChanged;
web_server.Server.onNameUpdated += Server_onNameUpdated;

void Server_onNameUpdated(object? sender, (string, string) e)
{
    edgeDeviceServer.UpdateAccount(e.Item1,e.Item2);
}

void Server_onSettingChanged(object? sender, string e)
{
    edgeDeviceServer.Update(e);
}

var webServerThread = new Thread(() => {
    web_server.Server.Launch(dataProvider);
});
webServerThread.IsBackground=false;
webServerThread.Start();

