using Microsoft.AspNetCore.Http.Json;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Text.Json;
using System.Text.Json.Serialization;
using System.Text.Json.Serialization.Metadata;
using System.Threading.Tasks;

namespace dorm_volume_server;

public record BaseJson
{
    static BaseJson()
    {
        var chain=jsonOptions.TypeInfoResolverChain;
        IJsonTypeInfoResolver[] chains = [
            BaseJsonContext.Default,
            RequestJsonContext.Default,
            VolumeJsonContext.Default,
            LoginJsonContext.Default
            ];
        foreach(var i in chains){
            chain.Insert(0, i);
        }
        
    }
    public static JsonSerializerOptions jsonOptions =new JsonSerializerOptions() { 
        IncludeFields=true,
        AllowTrailingCommas=true,
    };
    public string type { set; get; } = string.Empty;
}
[JsonSerializable(typeof(BaseJson))]
internal partial class BaseJsonContext : JsonSerializerContext
{

}
public record RequestJson:BaseJson
{
    public string content;
}
[JsonSerializable(typeof(RequestJson))]
internal partial class RequestJsonContext : JsonSerializerContext
{

}
public record VolumeJson : BaseJson
{
    public string volume_type;
    public float volume;
    public int time;
    public int dorm;
}
[JsonSerializable(typeof(VolumeJson))]
internal partial class VolumeJsonContext : JsonSerializerContext
{

}

public record LoginJson : BaseJson
{
    public string account;


}
[JsonSerializable(typeof(LoginJson))]
internal partial class LoginJsonContext : JsonSerializerContext
{

}