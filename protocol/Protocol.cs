﻿namespace Protocol;

public static class Client2Server
{
    public const string volume = "volume";
    public const string login = "schedule";
    public const string request = "request";
    public static class Request
    {
        public const string schedule = "schedule";

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
    }
}
public static class RESTfulAPI
{
    public static string Update_Account = "/update/account";
    public static string Update_Schedule = "/update/schedule";
    public static string Check_Account = "/check/account/";
}