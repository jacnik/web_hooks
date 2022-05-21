using LiteDB;
using OneOf;

namespace WebhookService.Registration;


internal record WebhookRegistrationResponse<T>
{
    internal struct Success{ public T Value {get; init;}}
    internal struct NotFound{}
    internal struct Error{ public Exception Reason {get; init;}}

    public OneOf<Success, NotFound, Error> Result {get; init;}
}

internal interface IWebhookRegistrationRepository
{
    public Task<WebhookRegistrationResponse<WebhookRegistered>> Insert(WebhookRegistered webhook, CancellationToken ct);
    public ValueTask<WebhookRegistrationResponse<WebhookRegistered>> GetById(Guid id, CancellationToken ct);
    public Task<WebhookRegistrationResponse<IEnumerable<WebhookRegistered>>> GetAll(/* page */CancellationToken ct);
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

    public async Task<WebhookRegistrationResponse<WebhookRegistered>> Insert(WebhookRegistered webhook, CancellationToken ct)
    {
        /* TODO constraint on registring two webhooks for the same trigger and url*/
        return await Task.Run(() => {
            try
            {
                this.registrationsCollection.Insert(webhook);
                return new WebhookRegistrationResponse<WebhookRegistered>
                {
                    Result = new WebhookRegistrationResponse<WebhookRegistered>.Success{Value = webhook}
                };
            }
            catch (Exception ex)
            {
                return new WebhookRegistrationResponse<WebhookRegistered>
                {
                    Result = new WebhookRegistrationResponse<WebhookRegistered>.Error{Reason = ex}
                };
            }
        }, ct);
    }

    public async ValueTask<WebhookRegistrationResponse<WebhookRegistered>> GetById(Guid id, CancellationToken ct)
    {
        return await Task.Run<WebhookRegistrationResponse<WebhookRegistered>>(() => {
            try
            {
                var dbRsp = this.registrationsCollection.Query()
                    .Where(r => r.EventId == id)
                    .Select(r => r)
                    .Limit(1)
                    .ToList();

                return new WebhookRegistrationResponse<WebhookRegistered>
                {
                    Result = dbRsp.Count == 0
                        ? new WebhookRegistrationResponse<WebhookRegistered>.NotFound()
                        : new WebhookRegistrationResponse<WebhookRegistered>.Success{Value = dbRsp.First()}
                };
            }
            catch (Exception ex)
            {
                return new WebhookRegistrationResponse<WebhookRegistered>
                {
                    Result = new WebhookRegistrationResponse<WebhookRegistered>.Error{Reason = ex}
                };
            }
        }, ct);
    }

    public async Task<WebhookRegistrationResponse<IEnumerable<WebhookRegistered>>> GetAll(/* page */CancellationToken ct)
    {
        return await Task.Run(() =>
        {
            try
            {
                var dbRsp = this.registrationsCollection.Query()
                    .OrderByDescending(r => r.CreatedAt)
                    .Select(r => r)
                    .Limit(10)
                    .ToEnumerable();

                return new WebhookRegistrationResponse<IEnumerable<WebhookRegistered>>
                {
                    Result = new WebhookRegistrationResponse<IEnumerable<WebhookRegistered>>.Success{Value = dbRsp}
                };
            }
            catch (Exception ex)
            {
                return new WebhookRegistrationResponse<IEnumerable<WebhookRegistered>>()
                {
                    Result = new WebhookRegistrationResponse<IEnumerable<WebhookRegistered>>.Error{Reason = ex}
                };
            }

        }, ct);
    }
}