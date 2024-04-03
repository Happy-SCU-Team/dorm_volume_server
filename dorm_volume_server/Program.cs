//launch web server

using DataProvider;
using EventManagerLib;

bool isDebug = true;

var dataProvider=new SimpleDataProvider(isDebug: isDebug);
if (isDebug)
{
    EventManager.DisplayLevel = Level.Info;
}


Console.WriteLine("---Accounts---");
foreach(var i in dataProvider.GetAccounts())
{
    Console.WriteLine(i);
}
Console.WriteLine("--------------");

var webServerThread = new Thread(() => {
    web_server.Server.Launch(dataProvider);
});
webServerThread.IsBackground=false;
webServerThread.Start();

