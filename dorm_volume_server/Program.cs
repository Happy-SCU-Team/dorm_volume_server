//launch web server

using DataProvider;

var dataProvider=new SimpleDataProvider();

//add test member
dataProvider.addTestMember();

var webServerThread = new Thread(() => {
    web_server.Server.Launch(dataProvider);
});
webServerThread.IsBackground=false;
webServerThread.Start();