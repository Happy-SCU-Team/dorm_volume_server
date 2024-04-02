//launch web server

using DataProvider;

var dataProvider=new SimpleDataProvider(isDebug:true);



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