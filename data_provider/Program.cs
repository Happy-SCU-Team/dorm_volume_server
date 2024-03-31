

using data_provider;

namespace DataProvider;

public abstract class DataProvider
{
    public static DataProvider Instance { get; }
    public abstract void RecordVolumeInfo(VolumeInfo volumeInfo);
    public abstract void UpdateScheduleSegment(ScheduleSegment[] scheduleSegment);
    public bool UpdateAccountName(string account, string new_name)
    {
        if (CheckAccountExistance(new_name))
        {
            updateAccountNameDirectly(account, new_name);
            return false;
        }
        return true;
    }


    public abstract bool CheckAccountExistance(string account);

    public abstract ScheduleSegment[] GetScheduleSegments(string account);
    private DataProvider(){}

    //----------private------------
    protected abstract void updateAccountNameDirectly(string account, string new_name);
}