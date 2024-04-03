using Microsoft.AspNetCore.Http.Json;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Text.Json;
using System.Text.Json.Serialization;
using System.Threading.Tasks;

namespace dorm_volume_server;

public record BaseJson
{
    static BaseJson()
    {
        jsonOptions.TypeInfoResolverChain.Insert(0,BaseJsonContext.Default);
    }
    public static JsonSerializerOptions jsonOptions =new JsonSerializerOptions() { 
        IncludeFields=true,
    };
    public string type { set; get; } = string.Empty;
}
[JsonSerializable(typeof(BaseJson))]
internal partial class BaseJsonContext : JsonSerializerContext
{

}
