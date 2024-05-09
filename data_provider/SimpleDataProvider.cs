using System.Runtime.InteropServices;
using System.Text.Json;
using System.Text.Json.Serialization;

namespace DataProvider;

public class SimpleDataProvider : DataProvider
{
    public SimpleDataProvider(bool isDebug) {
        options.TypeInfoResolverChain.Insert(0,DataContext.Default);
        try
        {
            load();
            //fixWrongTime();
        }
        catch
        {

        }
        
        if (isDebug)
        {
            this.addTestMember();
        }
    } 
    public string FileName { set; get; } = "database.json";
    private async Task<string> read(string file_name)
    {
        string result;
        using (var outputFile = new  StreamReader(file_name))
        {
            result= await outputFile.ReadToEndAsync();
        }
        return result;
    }
    private readonly JsonSerializerOptions options = new JsonSerializerOptions()
    {
        IncludeFields = true,
    };
    private Task? lastFileSaveTask;
    public async Task save()
    {
        var result=JsonSerializer.Serialize(data, options);
        await using FileStream createStream = File.Create(FileName);
        if (lastFileSaveTask == null || lastFileSaveTask.IsCompleted)
        {
            lastFileSaveTask = JsonSerializer.SerializeAsync(createStream, data, options);
            await lastFileSaveTask;
        }
    }
    public void load()
    {
        var result = read(FileName).Result!;
        data=JsonSerializer.Deserialize<Dictionary<string, SimpleIndividualData>>
            (result,options)!;
    }
    
    private Dictionary<string, SimpleIndividualData> data = new();
    public override bool CheckAccountExistance(string account)
    {
        return (data.ContainsKey(account));

    }

    public override IEnumerable<string> GetAccounts()
    {
        return data.Keys;
    }

    public override ScheduleSegment[] GetScheduleSegments(string account)
    {
        return data[account].scheduleSegments;
    }

    public override void RecordVolumeInfo(string account, VolumeInfo volumeInfo)
    {
        data[account].volumeInfos.Add(volumeInfo);
        save();
    }


    protected override void registerDirectly(string account)
    {
        data.Add(account, new SimpleIndividualData());
        save();
    }

    protected override void updateAccountNameDirectly(string account, string new_name)
    {

        data.Add(new_name, data[account]);
        data.Remove(account);
        save();
    }

    protected override void UpdateScheduleSegmentDirectly(string account, ScheduleSegment[] scheduleSegment)
    {

        data[account].scheduleSegments = scheduleSegment;
        save();
    }

    public override void UpdateInterval(string account, int interval)
    {
        data[account].interval = interval;
        save();
    }

    public override int getInterval(string account)
    {
        return data[account].interval;
    }

    public override IEnumerable<VolumeInfo> GetVolumeInfo(string account)
    {
        return data[account].volumeInfos;
    }
    [Obsolete]
    public async void fixWrongTime()
    {
        _modify();
        await save();
        Console.WriteLine("Fix wrong time");
    }
    private void _modify()
    {
        foreach (var i in data.Values)
        {
            Span<VolumeInfo> volumeinfos = CollectionsMarshal.AsSpan(i.volumeInfos);
            for(int j=0; j < volumeinfos.Length; j++)
            {
                var num= volumeinfos[j].time.Ticks;
                var startTime = TimeZone.CurrentTimeZone.ToLocalTime(new DateTime(1970, 1, 1));//当地时区
                volumeinfos[j].time=startTime.AddSeconds(num);
            }
        }
    }
}

class SimpleIndividualData
{
    public int interval=30;
    public List<VolumeInfo> volumeInfos = new List<VolumeInfo>();
    public ScheduleSegment[] scheduleSegments = Array.Empty<ScheduleSegment>();
}
[JsonSerializable(typeof(SimpleIndividualData))]
internal partial class SimpleIndividualDataContext : JsonSerializerContext
{

}

[JsonSerializable(typeof(ScheduleSegment[]))]
internal partial class ScheduleSegmentsContext : JsonSerializerContext
{

}
[JsonSerializable(typeof(Dictionary<string, SimpleIndividualData>))]
internal partial class DataContext : JsonSerializerContext
{

}