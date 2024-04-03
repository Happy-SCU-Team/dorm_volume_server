using DataProvider;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using EventManagerLib;

namespace dorm_volume_server;

internal static class QQ_Alert
{
    public static string URL { set; get; } = "http://localhost/alert"; 
    /// <summary>
    /// invoke QQ interface
    /// </summary>
    /// <param name="account"></param>
    /// <param name="volumeInfo"></param>
    public static async void alert(string account,VolumeInfo volumeInfo)
    {
        EventManagerLib.EventManager.Add("alerting via QQ");
        string jsonPayload = $"{{\"account\":{account},\"dorm_number\": {volumeInfo.dormId}}}";
        using (HttpClient client = new HttpClient())
        {
            HttpResponseMessage response =
                await client.PostAsync(
                    URL,new StringContent(jsonPayload, Encoding.UTF8, "application/json")
                );
            if(!response.IsSuccessStatusCode) {
                EventManagerLib.EventManager.Add(EventManagerLib.Level.Warn,"alert via QQ failed");
            }
        }
    }
}
