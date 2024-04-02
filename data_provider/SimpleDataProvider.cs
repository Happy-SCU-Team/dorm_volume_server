using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace DataProvider;

public class SimpleDataProvider : DataProvider
{
    private Dictionary<string, SimpleIndividualData> data = new();
    public override bool CheckAccountExistance(string account)
    {
        return (data.ContainsKey(account));

    }

    public override ScheduleSegment[] GetScheduleSegments(string account)
    {
        return data[account].scheduleSegments;
    }

    public override void RecordVolumeInfo(string account, VolumeInfo volumeInfo)
    {
        data[account].volumeInfos.Add(volumeInfo);
    }


    protected override void registerDirectly(string account)
    {
        data.Add(account, new SimpleIndividualData());
    }

    protected override void updateAccountNameDirectly(string account, string new_name)
    {
        data.Add(new_name, data[account]);
        data.Remove(account);
    }

    protected override void UpdateScheduleSegmentDirectly(string account, ScheduleSegment[] scheduleSegment)
    {
        data[account].scheduleSegments = scheduleSegment;
    }
}

class SimpleIndividualData
{
    public List<VolumeInfo> volumeInfos = new List<VolumeInfo>();
    public ScheduleSegment[] scheduleSegments = Array.Empty<ScheduleSegment>();
}
