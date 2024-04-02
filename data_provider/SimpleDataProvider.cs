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
    public async void save()
    {
        await using FileStream createStream = File.Create(FileName);
        await JsonSerializer.SerializeAsync(createStream, data,options);
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
}

class SimpleIndividualData
{
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