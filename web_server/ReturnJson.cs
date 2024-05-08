using DataProvider;
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


[JsonSerializable(typeof(IEnumerable<VolumeInfo>))]
internal partial class VolumeInfosJsonContext : JsonSerializerContext
{

}
public struct VolumeInfoSend
{
    public long time;
    public string volume_type;
    public float volume;
    public int dorm_id;
    public VolumeInfoSend(VolumeInfo volumeInfo) {
        time = (long)(volumeInfo.time - new DateTime(1970, 1, 1, 0, 0, 0, 0)).TotalSeconds;
        volume_type = volumeInfo.volumeType.ToString();
        volume=volumeInfo.volume;
        dorm_id=volumeInfo.dormId;
    }
}
[JsonSerializable(typeof(IEnumerable<VolumeInfoSend>))]
internal partial class VolumeInfoSendJsonContext : JsonSerializerContext
{

}