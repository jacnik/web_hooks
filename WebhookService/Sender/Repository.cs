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
    public Task<WebhookSenderResponse> GetForTrigger(string trigger);
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
            try
            {
                var dbRsp = this.webhooksCollection.Query()
                    .Where(r => r.Webhook.Trigger == trigger)
                    .Select(r => r)
                    .ToArray();

                return new WebhookSenderResponse
                {
                    Result = new WebhookSenderResponse.Success{Value = dbRsp}
                };
            }
            catch (Exception ex)
            {
                return new WebhookSenderResponse
                {
                    Result = new WebhookSenderResponse.Error{Reason = ex}
                };
            }
        });
    }
}
