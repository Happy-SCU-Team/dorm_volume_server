using System.Text.Json.Serialization;
namespace web_server;


public class Server
{
    public static void Launch(DataProvider.DataProvider provider)
    {
        var builder = WebApplication.CreateSlimBuilder();

        builder.Services.ConfigureHttpJsonOptions(options =>
        {
            var chain=options.SerializerOptions.TypeInfoResolverChain;
            chain.Insert(0, UpdateAccountNameContext.Default);
            chain.Insert(0, UpdateScheduleContext.Default);
            chain.Insert(0,SegmentContext.Default);

        });

        var app = builder.Build();
        //update
        var updateApi = app.MapGroup("/update");
        updateApi.MapPost("/account", (UpdateAccountName updateAccount) => {
            provider.UpdateAccountName(updateAccount.account,updateAccount.new_account);
        });
        updateApi.MapPost("/schedule", (UpdateSchedule updateSchedule) => {
            provider.UpdateScheduleSegment(updateSchedule.account,updateSchedule.segments);
        });

        //check
        var checkApi = app.MapGroup("/check");
        checkApi.MapGet("/account/[q]",(string name) =>{
            return provider.CheckAccountExistance(name);
        });


        app.Run();
    }
}




