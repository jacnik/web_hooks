using LiteDB;

namespace WebhookService.Registration;


internal interface IWebhookRegistrationRepository
{
    public Task /*TODO return type */ Insert(WebhookRegistered webhook, CancellationToken ct);
    public ValueTask<WebhookRegistered?> /*TODO return type */ GetById(Guid id, CancellationToken ct);
    public Task<IEnumerable<WebhookRegistered>> /*TODO return type */ GetAll(/* page */CancellationToken ct);
}

internal class WebhookRegistrationRepository : IWebhookRegistrationRepository
{
    private readonly ILiteCollection<WebhookRegistered> registrationsCollection;

    public WebhookRegistrationRepository(ILiteCollection<WebhookRegistered> registrationsCollection)
    {
        registrationsCollection.EnsureIndex(p => p.EventId);
        registrationsCollection.EnsureIndex(p => p.CreatedAt);
        registrationsCollection.EnsureIndex(p => p.Webhook.Trigger);
        this.registrationsCollection = registrationsCollection;
    }

    public async Task /*TODO return type */ Insert(WebhookRegistered webhook, CancellationToken ct)
    {
        /* TODO constraint on registring two webhooks for the same trigger and url*/
        await Task.Run(() => this.registrationsCollection.Insert(webhook), ct);
    }

    public async ValueTask<WebhookRegistered?> /*TODO return type */ GetById(Guid id, CancellationToken ct)
    {
        return await Task.Run<WebhookRegistered?>(() => {
            /* TODO try catch */
            var dbRsp = this.registrationsCollection.Query()
                .Where(r => r.EventId == id)
                .Select(r => r)
                .Limit(1)
                .ToList();

            return dbRsp.Count != 0 ? dbRsp.First() : null;
        }, ct);
    }

    public async Task<IEnumerable<WebhookRegistered>> /*TODO return type */ GetAll(/* page */CancellationToken ct)
    {
        /* TODO try catch */
        return await Task.Run(() =>
        {
            var dbRsp = this.registrationsCollection.Query()
                .OrderByDescending(r => r.CreatedAt)
                .Select(r => r)
                .Limit(10)
                .ToEnumerable();

            return dbRsp;
        }, ct);
    }
}