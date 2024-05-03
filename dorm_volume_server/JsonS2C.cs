using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Text.Json.Serialization.Metadata;
using System.Text.Json.Serialization;
using System.Text.Json;
using System.Threading.Tasks;
using Protocol;
using DataProvider;

namespace Server2ClientJson;

public record BaseJson
{
    static BaseJson()
    {
        var chain = jsonOptions.TypeInfoResolverChain;
        IJsonTypeInfoResolver[] chains = [
            BaseJsonContext2.Default,
            UpdateJsonContext2.Default,
            ScheduleUpdateJsonContext2.Default,
            AccountUpdateJsonContext2.Default,
            IntervalUpdateJsonContext2.Default,
            ScheduleUpdateJson_obsoleteContext2.Default,
            ];
        foreach (var i in chains)
        {
            chain.Insert(0, i);
        }

    }
    public static JsonSerializerOptions jsonOptions = new JsonSerializerOptions()
    {
        IncludeFields = true,
        AllowTrailingCommas = true,
        
    };
    public string type { set; get; } = string.Empty;
}

[JsonSerializable(typeof(BaseJson))]
internal partial class BaseJsonContext2 : JsonSerializerContext
{

}

public record UpdateJson:BaseJson
{
    public UpdateJson()
    {
        type = Server2Client.update;
    }
    public string content;
}
[JsonSerializable(typeof(UpdateJson))]
internal partial class UpdateJsonContext2 : JsonSerializerContext
{

}

public record ScheduleUpdateJson_new : UpdateJson
{
    public ScheduleUpdateJson_new()
    {
        content=Server2Client.Update.schedule;
    }
    public ScheduleSegment[] schedule;
}

[JsonSerializable(typeof(ScheduleUpdateJson_new))]
internal partial class ScheduleUpdateJsonContext2 : JsonSerializerContext
{

}
public record ScheduleUpdateJson_obsolete : UpdateJson
{

    public ScheduleUpdateJson_obsolete(ScheduleUpdateJson_new scheduleUpdateJson_NEW)
    {
        content = Server2Client.Update.schedule;
        schedule=new ScheduleSegmentObsolete[scheduleUpdateJson_NEW.schedule.Length];
        for(int i = 0; i < scheduleUpdateJson_NEW.schedule.Length; i++)
        {
            int hour_s = scheduleUpdateJson_NEW.schedule[i].start_time / 60;
            int min_s = scheduleUpdateJson_NEW.schedule[i].start_time % 60;

            int hour_e = scheduleUpdateJson_NEW.schedule[i].end_time / 60;
            int min_e = scheduleUpdateJson_NEW.schedule[i].end_time % 60;
            schedule[i] = new ScheduleSegmentObsolete() {
                start_time = $"{hour_s}:{min_s}",
                end_time = $"{hour_e}:{min_e}"
            };
        }
    }
    public ScheduleSegmentObsolete[] schedule;
}
public struct ScheduleSegmentObsolete
{
    public string start_time;
    public string end_time;
}
[JsonSerializable(typeof(ScheduleUpdateJson_obsolete))]
internal partial class ScheduleUpdateJson_obsoleteContext2 : JsonSerializerContext
{

}


public record AccountUpdateJson : UpdateJson
{
    public AccountUpdateJson()
    {
        content = Server2Client.Update.account;
    }
    public string account;
}
[JsonSerializable(typeof(AccountUpdateJson))]
internal partial class AccountUpdateJsonContext2 : JsonSerializerContext
{

}
public record IntervalUpdateJson : UpdateJson
{
    public IntervalUpdateJson()
    {
        content = Server2Client.Update.interval;
    }
    public int interval;
}
[JsonSerializable(typeof(IntervalUpdateJson))]
internal partial class IntervalUpdateJsonContext2 : JsonSerializerContext
{

}
public record MuteJson : BaseJson
{
    public MuteJson()
    {
        type=Server2Client.mute;
    }
    public string time;
}
[JsonSerializable(typeof(MuteJson))]
internal partial class MuteJsonContext2 : JsonSerializerContext
{

}