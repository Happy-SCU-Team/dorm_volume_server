using Microsoft.AspNetCore.Mvc;
using System.Text.Json.Serialization;
using Protocol;

namespace web_server;


public class Server
{
    public static void Launch(DataProvider.DataProvider provider)
    {
        var builder = WebApplication.CreateSlimBuilder();

        builder.Services.ConfigureHttpJsonOptions(options =>
        {
            options.SerializerOptions.IncludeFields = true;

            var chain=options.SerializerOptions.TypeInfoResolverChain;
            chain.Insert(0, UpdateAccountNameContext.Default);
            chain.Insert(0, UpdateScheduleContext.Default);
            chain.Insert(0,SegmentContext.Default);
            chain.Insert(0,isFailedJsonContext.Default);
            chain.Insert(0,IsExistJsonContext.Default);

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




        app.Run();
    }
}




