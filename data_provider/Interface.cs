



namespace DataProvider;

public abstract class DataProvider
{
    public static DataProvider Instance { get; }
    public abstract void RecordVolumeInfo(string account,VolumeInfo volumeInfo);
    public bool UpdateScheduleSegment(string account,ScheduleSegment[] scheduleSegment)
    {
        int max_time = 7 * 24 *60;
        bool flag = scheduleSegment[0].start>=0;
        flag &= scheduleSegment[scheduleSegment.Length-1].end<max_time;
        for(int i = 0;i<scheduleSegment.Length-1 && flag;i++)
        {
            flag &= scheduleSegment[i].start<scheduleSegment[i].end;
            flag &= scheduleSegment[i].end < scheduleSegment[i+1].start;
        }
        if(flag)
        {
            UpdateScheduleSegmentDirectly(account,scheduleSegment);
            return true;
        }
        return false;
        
    }
    public bool UpdateAccountName(string account, string new_name)
    {
        if (CheckAccountExistance(new_name))
        {
            updateAccountNameDirectly(account, new_name);
            return false;
        }
        return true;
    }
    public bool register(string account)
    {
        if (CheckAccountExistance(account))
        {
            return false;
        }
        else
        {
            registerDirectly(account);
            return true;
        }

    }


    public abstract bool CheckAccountExistance(string account);

    public abstract ScheduleSegment[] GetScheduleSegments(string account);
    protected DataProvider(){}

    //----------private------------
    protected abstract void updateAccountNameDirectly(string account, string new_name);
    protected abstract void registerDirectly(string account);
    protected abstract void UpdateScheduleSegmentDirectly(string account, ScheduleSegment[] scheduleSegment);
}