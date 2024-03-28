

using data_provider;

namespace DataProvider;

public abstract class DataProvider
{
    public static DataProvider Instance { get; }
    public abstract void RecordVolumeInfo(VolumeInfo volumeInfo);
    private DataProvider()
    {

    }
}