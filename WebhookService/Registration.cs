using LiteDB;

namespace WebhookService.Registration;

internal readonly struct RegisterWebhookCommand /* TODO ICommand instead of ...Command name */
{
    public string Url {get; init;}
    public string Trigger {get; init;} // TODO Enum actions
    public string Secret {get; init;}
    public IDictionary<string, string> AdditionalHeaders {get; init;}
    /* TODO owner */
}

internal class WebhookRegistered /*TODO struct*/
{
    public Guid EventId {get; init;} /* TODO create WebhookRegisteredId type */
    public DateTime CreatedAt {get; init;}
    public RegisterWebhookCommand Webhook {get; init;}
}

internal static class WebhookRegistrationRestEndpoint
{
    internal static async Task<IResult> PostWebhook(RegisterWebhookCommand cmd, IWebhookRegistrationRepository db, CancellationToken cancellationToken)
    {
        var webhookRegistered = new WebhookRegistered
            {
                CreatedAt = DateTime.UtcNow,
                EventId = Guid.NewGuid(),
                Webhook = cmd
            };
        await db.Insert(webhookRegistered);

        return Results.Created($"/webhooks/{webhookRegistered.EventId}", webhookRegistered);
    }
    internal static async Task<IResult> GetWebhooks(IWebhookRegistrationRepository db, CancellationToken cancellationToken) => Results.Ok(await db.GetAll());
    internal static async Task<IResult> GetWebhookById(Guid id, IWebhookRegistrationRepository db, CancellationToken cancellationToken)
    {
        return await db.GetById(id) is WebhookRegistered webhook
            ? Results.Ok(webhook)
            : Results.NotFound();
    }

}

internal interface IWebhookRegistrationRepository
{
    public Task /*TODO return type */ Insert(WebhookRegistered webhook);
    public ValueTask<WebhookRegistered> /*TODO return type */ GetById(Guid id);
    public Task<IEnumerable<WebhookRegistered>> /*TODO return type */ GetAll(/* page */);
}

internal class WebhookRegistrationRepository : IWebhookRegistrationRepository
{
    private readonly LiteDatabase db;
    private readonly ILiteCollection<WebhookRegistered> registrationsCollection;

    public WebhookRegistrationRepository(LiteDatabase db)
    {
        this.db = db;

        var registrationsCollection = db.GetCollection<WebhookRegistered>("WebhookRegistrations");
        registrationsCollection.EnsureIndex(p => p.EventId);
        registrationsCollection.EnsureIndex(p => p.CreatedAt);
        registrationsCollection.EnsureIndex(p => p.Webhook.Trigger);
        this.registrationsCollection = registrationsCollection;
    }

    public async Task /*TODO return type */ Insert(WebhookRegistered webhook)
    {
        await Task.Run(() => this.registrationsCollection.Insert(webhook));
    }

    public async ValueTask<WebhookRegistered> /*TODO return type */ GetById(Guid id)
    {
        /* TODO try catch */
        var dbRsp = this.registrationsCollection.Query()
            .Where(r => r.EventId == id)
            .Select(r => r)
            .ToList();

        /* Forcing Task to compensate for lack of async/await in LiteDb */
        return await Task.FromResult(dbRsp.FirstOrDefault());
    }

    public async Task<IEnumerable<WebhookRegistered>> /*TODO return type */ GetAll(/* page */)
    {
        /* TODO try catch */

        var dbRsp = this.registrationsCollection.Query()
            .OrderBy(r => r.CreatedAt)
            .Select(r => r)
            .Limit(10)
            .ToEnumerable();

        return await Task.FromResult(dbRsp);
    }
}