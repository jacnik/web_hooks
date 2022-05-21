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
        WebhookSender sender,
        CancellationToken ct)
    {
        await sender.OnActionHappened(action);
        return Results.Ok();
    }
}

internal class WebhookSender
{
    private readonly IWebhookSenderRepository repo;
    public WebhookSender(IWebhookSenderRepository repo)
    {
        this.repo = repo;
    }

    public async Task /* TODO return RequestAccepted obj*/ OnActionHappened(ActionEvent action)
    {
        var triggersRsp = await repo.GetForTrigger(action.EventName);
        await triggersRsp.Result.Match(
            async success => {
                using (var client = new HttpClient())
                {
                    foreach (var webhook in success.Value)
                    {
                        var httpRsp = await client.PostAsync(
                            webhook.Webhook.Url,
                            new StringContent(webhook.Webhook.Content));

                    }
                }

                await Task.CompletedTask;
            },
            error => Task.CompletedTask // TODO return error
        );
    }
}