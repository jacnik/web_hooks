using System.Threading.Channels;
using WebhookService.Registration;
using OneOf;
using LiteDB;

namespace WebhookService.Sender;


internal struct WebhookSenderResponse
{
    internal struct Success{ public IEnumerable<WebhookRegistered> Value {get; init;}}
    internal struct Error{ public Exception Reason {get; init;}}

    public OneOf<Success, Error> Result {get; init;}
}

internal interface IWebhookSenderRepository
{
    public Task<WebhookSenderResponse> GetForTrigger(string trigger); /*TODO paging*/
}

internal class WebhookSenderRepository : IWebhookSenderRepository
{
    private readonly ILiteCollection<WebhookRegistered> webhooksCollection;

    public WebhookSenderRepository(ILiteCollection<WebhookRegistered> webhooksCollection)
    {
        webhooksCollection.EnsureIndex(p => p.Webhook.Trigger);
        this.webhooksCollection = webhooksCollection;
    }

    public async Task<WebhookSenderResponse> GetForTrigger(string trigger)
    {
        return await Task.Run(() =>
        {
            var dbRsp = this.webhooksCollection.Query()
                .Where(r => r.Webhook.Trigger == trigger)
                .Select(r => r)
                .ToArray();

            return new WebhookSenderResponse
            {
                Result = new WebhookSenderResponse.Success{Value = dbRsp}
            };
        });
    }
}


internal struct ActionEvent
{
    /**
    This is supposed to simulate a event happened in system.
    It should trigger webhook sending.
    */
    public string EventName {get; init;}
}

internal static class WebhookSenderRestApi
{
    internal static async Task<IResult> PostActionEvent(
        ActionEvent action,
        WebhookScheduler scheduler,
        CancellationToken ct)
    {
        await scheduler.OnActionHappened(action);
        return Results.Ok();
    }
}

internal class WebhookSender : BackgroundService
{
    private readonly Channel<ActionEvent> actions;
    private readonly IWebhookSenderRepository repo;
    private readonly ILogger<WebhookSender> logger;


    public WebhookSender(
        Channel<ActionEvent> actions,
        IWebhookSenderRepository repo,
        ILogger<WebhookSender> logger)
    {
        this.actions = actions;
        this.repo = repo;
        this.logger = logger;
    }

    public async Task OnEvent(ActionEvent action)
    {
        var triggersRsp = await repo.GetForTrigger(action.EventName);
        await triggersRsp.Result.Match(
            async success => {
                /* TODO interface */
                using (var client = new HttpClient())
                {
                    foreach (var webhook in success.Value)
                    {
                        var httpRsp = await client.PostAsync(
                            webhook.Webhook.Url,
                            new StringContent(webhook.Webhook.Content));
                    }
                }
            },
            error => Task.Run(() => {
                logger.LogInformation("Queued Hosted Service is stopping.");
            })
        );
    }

    protected override async Task ExecuteAsync(CancellationToken ct)
    {
        while (await this.actions.Reader.WaitToReadAsync(ct))
        {
            var action = await this.actions.Reader.ReadAsync(ct);
            await OnEvent(action);
        }
    }

    public override async Task StopAsync(CancellationToken ct)
    {
        logger.LogInformation("Queued Hosted Service is stopping.");

        await base.StopAsync(ct);
    }
}

internal class WebhookScheduler
{
    private readonly Channel<ActionEvent> actions;
    public WebhookScheduler(Channel<ActionEvent> actions)
    {
        this.actions = actions;
    }

    public async Task /* TODO return RequestAccepted obj*/ OnActionHappened(ActionEvent action)
    {
        await actions.Writer.WriteAsync(action);
    }
}