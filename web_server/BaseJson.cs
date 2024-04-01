using System.Text.Json.Serialization;

namespace web_server;

public record BaseJson
{
    public string account=string.Empty;
}
[JsonSerializable(typeof(BaseJson))]
internal partial class BaseJsonContext : JsonSerializerContext
{

}

public record UpdateAccountName : BaseJson
{
    public string new_account;
}
[JsonSerializable(typeof(UpdateAccountName))]
internal partial class UpdateAccountNameContext : JsonSerializerContext
{

}

public record UpdateSchedule : BaseJson
{
    public DataProvider.ScheduleSegment[] segments;
}
[JsonSerializable(typeof(UpdateSchedule))]
internal partial class UpdateScheduleContext : JsonSerializerContext
{

}



[JsonSerializable(typeof(DataProvider.ScheduleSegment[]))]
internal partial class SegmentContext : JsonSerializerContext
{

}