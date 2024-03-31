using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace data_provider;

public class DataTypes
{
    public string Account { get; set; }
}

public enum VolumeType { alert,info}
public struct VolumeInfo
{
    public string account;
    public float volume;
    public VolumeType volumeType;
    public int time;
    public int dormId;
}
public struct ScheduleSegment
{//start,end以分钟为单位，以每一周的第一天为0点
    public int start;
    public int end;
}