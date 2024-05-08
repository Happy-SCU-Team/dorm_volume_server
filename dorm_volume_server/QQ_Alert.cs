using DataProvider;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using EventManagerLib;

namespace dorm_volume_server;

public static class QQ_Alert
{
    public static string URL { set; get; } = "http://192.168.243.214:5000/alert"; 
    /// <summary>
    /// invoke QQ interface
    /// </summary>
    /// <param name="account"></param>
    /// <param name="volumeInfo"></param>
    public static async Task alert(string account,VolumeInfo volumeInfo)
    {
        EventManagerLib.EventManager.Add("alerting via QQ");
        string jsonPayload = $"{{\"account\":\"{account}\",\"dorm_number\": {volumeInfo.dormId}}}";
        using (HttpClient client = new HttpClient())
        {
            HttpResponseMessage response =
                await client.PostAsync(
                    URL,new StringContent(jsonPayload, Encoding.UTF8, "application/json")
                );
            Console.WriteLine(response.StatusCode);
            if(!response.IsSuccessStatusCode) {
                EventManagerLib.EventManager.Add(EventManagerLib.Level.Warn,"alert via QQ failed");
            }
            else
            {
                EventManager.Add(Level.Info, "alert via QQ successful");
            }
        }
    }
}
