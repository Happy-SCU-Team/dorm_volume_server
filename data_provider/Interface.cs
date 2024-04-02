



namespace DataProvider;

public abstract class DataProvider
{
    public event EventHandler<SettingChangedEvent> OnSettingChanged;
    protected void InvokeSettingChanged(SettingChangedEvent @event)
    {
        OnSettingChanged.Invoke(this, @event);
    }
    protected void InvokeSettingChanged(string account,SettingChangedType type)
    {
        InvokeSettingChanged(new SettingChangedEvent(account, type));
    }
    public abstract void RecordVolumeInfo(string account,VolumeInfo volumeInfo);
    public bool UpdateScheduleSegment(string account,ScheduleSegment[] scheduleSegment)
    {
        if (scheduleSegment.Length==0)
        {
            return false;
        }
        int max_time = 7 * 24 *60;
        bool flag = scheduleSegment[0].start_time>=0;
        flag &= scheduleSegment[scheduleSegment.Length-1].end_time<max_time;
        for(int i = 0;i<scheduleSegment.Length-1 && flag;i++)
        {
            flag &= scheduleSegment[i].start_time<scheduleSegment[i].end_time;
            flag &= scheduleSegment[i].end_time < scheduleSegment[i+1].start_time;
        }
        if(flag)
        {
            InvokeSettingChanged(new SettingChangedEvent(account,SettingChangedType.segments_modify));
            UpdateScheduleSegmentDirectly(account,scheduleSegment);
            return true;
        }
        return false;
        
    }
    public string? UpdateAccountName(string account, string new_name)
    {
        if(string.IsNullOrEmpty(new_name)) {
            return "This name is invalid";
        }
        if (CheckAccountExistance(new_name))
        {
            return "this name already exists";
        }
        InvokeSettingChanged(account, SettingChangedType.name_modify);
        updateAccountNameDirectly(account, new_name);
        return null;
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
    public DataProvider(){}
    public void addTestMember()
    {
        register("西苑6舍五单元101");
        register("西苑6舍五单元102");
        register("西苑6舍五单元103");
        register("west_dorm");
    }

    //----------private------------
    protected abstract void updateAccountNameDirectly(string account, string new_name);
    protected abstract void registerDirectly(string account);
    protected abstract void UpdateScheduleSegmentDirectly(string account, ScheduleSegment[] scheduleSegment);
}