/*
 * 实现功能：使用UTF-8格式编码，发送完后在末尾发送EOF字节 ascii值为4的字符
 * 
 * client->server
 * 
 *{
 *  "message":string,
 *  "number":int
 *}
 *
 *server->
 *{
 *  "message":string,
 *  "number":int
 *}
 * 
 */


using Conn;
using System.Text.Json;
using System.Text.Json.Serialization;

var jsonOptions = new JsonSerializerOptions()
{
    IncludeFields = true,
    AllowTrailingCommas = true,
};
jsonOptions.TypeInfoResolverChain.Insert(0,PackageContext.Default);

ConnectionServer server = new(12345, handler);

void handler(Connection connection)
{
    Console.WriteLine("连接成功");
    connection.messageHander = (m) => {
        try
        {
            Console.WriteLine("Data Received"+m);
            Package result = JsonSerializer.Deserialize<Package>(m,jsonOptions)!;
            result.number++;
            connection.send(JsonSerializer.Serialize(result,jsonOptions));
        }
        catch
        {
        }
    };
    connection.StartReceive();

}
server.Wait();
class Package
{
    public string message;
    public int number;
}
[JsonSerializable(typeof(Package))]
internal partial class PackageContext : JsonSerializerContext
{

}