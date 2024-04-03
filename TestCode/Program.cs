using System.Text.Json.Serialization;
using System.Text.Json;



string json = @"
{
    ""type"":""volume_info"",
    ""account"":123,
} 
";

BaseJson obj= JsonSerializer.Deserialize<BaseJson>(json,BaseJson.jsonOptions);

Console.WriteLine();
public record BaseJson
{
    static BaseJson()
    {
        jsonOptions.TypeInfoResolverChain.Insert(0, BaseJsonContext.Default);
    }
    public static JsonSerializerOptions jsonOptions = new JsonSerializerOptions()
    {
        IncludeFields = true,
        AllowTrailingCommas = true,
    };
    public string type { set; get; } = string.Empty;
}
[JsonSerializable(typeof(BaseJson))]
internal partial class BaseJsonContext : JsonSerializerContext
{

}

