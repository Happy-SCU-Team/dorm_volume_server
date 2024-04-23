using Microsoft.AspNetCore.Mvc;
using System.Text.Json.Serialization;
using Protocol;
using EventManagerLib;
using System.Runtime.CompilerServices;


namespace web_server;



public class Server
{
    public static event EventHandler<string>? onSettingChanged;
    public static event EventHandler<(string, string)>? onNameUpdated; 
    public static void Launch(DataProvider.DataProvider provider)
    {
        var builder = WebApplication.CreateSlimBuilder();

        builder.WebHost.UseUrls("http://0.0.0.0:5000");
        builder.Services.ConfigureHttpJsonOptions(options =>
        {
            options.SerializerOptions.IncludeFields = true;

            var chain=options.SerializerOptions.TypeInfoResolverChain;
            chain.Insert(0, UpdateAccountNameContext.Default);
            chain.Insert(0, UpdateScheduleContext.Default);
            chain.Insert(0,SegmentContext.Default);
            chain.Insert(0,isFailedJsonContext.Default);
            chain.Insert(0,IsExistJsonContext.Default);
            chain.Insert(0, UpdateIntervalContext.Default);

        });
        builder.Services.AddCors(options =>
        {
            options.AddPolicy(name: "allow all",
                              policy =>
                              {
                                  policy.AllowAnyOrigin().AllowAnyHeader();
                              });
        });

        var app = builder.Build();
        //is available
        app.MapGet("/",()=>"Server is running!!!");
        //app.MapGet("/echo/{value}",(string value)=>value);

        //check
        app.MapGet(RESTfulAPI.Check_Account, (string q) => {
            bool flag = provider.CheckAccountExistance(q);
            var msg = new IsExistJson(flag);
            return Results.Ok(msg);
        });

        //update
        app.MapPost(RESTfulAPI.Update_Account, (UpdateAccountName updateAccount) => {
            var flag=provider.UpdateAccountName(updateAccount.account, updateAccount.new_account);
            var msg = new IsFailedJson();
            msg.is_success = flag==null;
            if (!msg.is_success)
            {
                msg.failed_message = flag!;
            }
            onNameUpdated?.Invoke(null,(updateAccount.account,updateAccount.new_account));
            return Results.Ok(msg);
        });


        app.MapPost(RESTfulAPI.Update_Schedule, (UpdateSchedule updateSchedule) => {
            var flag=provider.UpdateScheduleSegment(updateSchedule.account,updateSchedule.segments);
            var msg = new IsFailedJson();
            msg.is_success = flag;
            if (!flag)
            {
                msg.failed_message = "the segment is invaild";
            }
            onSettingChanged?.Invoke(null,updateSchedule.account);
            return Results.Ok(msg);
        });
        app.MapPost(RESTfulAPI.Update_Interval,(UpdateInterval UpdateInterval) =>
        {
            var msg = new IsFailedJson();
            try
            {
                provider.UpdateInterval(UpdateInterval.account,UpdateInterval.interval);
                msg.is_success = true;
            }
            catch(Exception e)
            {
                msg.is_success = false;
                msg.failed_message=e.Message;
            }
            
            onSettingChanged?.Invoke(null, UpdateInterval.account);
            return Results.Ok(msg);
        });

        //get
        app.MapGet(RESTfulAPI.Get_Schedule,(string account) =>
        {
            try
            {
                return Results.Ok(provider.GetScheduleSegments(account));
            }
            catch
            {
                return Results.NotFound();
            }
        });
        app.MapGet(RESTfulAPI.Get_All_Account, () => { 
            return Results.Ok(provider.GetAccounts());
        });
        EventManager.Add("RESTful API is launching");
        app.Run();
    }
}




