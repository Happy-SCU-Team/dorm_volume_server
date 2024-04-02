using System.Text.Json.Serialization;

namespace web_server;

public record IsFailedJson 
{
    public bool is_success { set; get; }
    public string failed_message { set; get; } = string.Empty;
}
[JsonSerializable(typeof(IsFailedJson))]
internal partial class isFailedJsonContext : JsonSerializerContext
{

}

public record IsExistJson(bool is_exist);
[JsonSerializable(typeof(IsExistJson))]
internal partial class IsExistJsonContext : JsonSerializerContext
{

}
