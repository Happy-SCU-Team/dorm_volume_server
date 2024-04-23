namespace Protocol;

public static class Client2Server
{
    public const string volume = "volume";
    public const string login = "login";
    public const string request = "request";
    public static class Request
    {
        public const string schedule = "schedule";
    }
    public static class VolumeType
    {
        public const string alert = "alert";
        public const string info = "info";
    }
}

public static class Server2Client
{
    public const string update = "update";
    public const string mute="mute";
    public static class Update
    {
        public const string schedule = "schedule";
        public const string account = "account";
        public const string interval = "interval";
    }
}
public static class RESTfulAPI
{
    public const string Get_All_Account = "/all_account";
    public const string Update_Account = "/update/account";
    public const string Update_Schedule = "/update/schedule";
    public const string Check_Account = "/check/account/{q}";
    public const string Get_Schedule = "/{account}/schedule";
    public const string Update_Interval = "/update/interval";
}