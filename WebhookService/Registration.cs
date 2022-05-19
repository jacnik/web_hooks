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

internal class WebhookRegistered /*TODO truct*/
{
    public Guid EventId {get; init;} /* TODO create WebhookRegisteredId type */
    public DateTime CreatedAt {get; init;}
    public RegisterWebhookCommand Webhook {get; init;}
}

internal class WebhookRegistration
{

}

internal interface IWebhookRegistrationRepository
{
    public void /*TODO return type */ Insert(WebhookRegistered webhook);
    public WebhookRegistered /*TODO return type */ GetById(Guid id);
    public IEnumerable<WebhookRegistered> /*TODO return type */ GetAll(/* page */);
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

    public void /*TODO return type */ Insert(WebhookRegistered webhook)
    {
        this.registrationsCollection.Insert(webhook);
    }

    public WebhookRegistered /*TODO return type */ GetById(Guid id)
    {
        // var results = priceCollection.Query()
        //     .Where(p => p.Symbol == "MSFT")
        //     .Where(p => p.Interval == "M1")
        //     .OrderBy(p => p.ValidFrom)
        //     .Select(p => p)
        //     .Limit(10)
        //     .ToList();

        /* TODO try catch */
        var dbRsp = this.registrationsCollection.Query()
            .Where(r => r.EventId == id)
            .Select(r => r)
            .ToList();

        return dbRsp.FirstOrDefault();
    }

    public IEnumerable<WebhookRegistered> /*TODO return type */ GetAll(/* page */)
    {
        /* TODO try catch */

        var dbRsp = this.registrationsCollection.Query()
            .OrderBy(r => r.CreatedAt)
            .Select(r => r)
            .Limit(10)
            .ToEnumerable();

        return dbRsp;
    }
}