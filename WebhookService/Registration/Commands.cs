namespace WebhookService.Registration;

internal readonly struct RegisterWebhookCommand
{
    public string Url {get; init;}
    public string Trigger {get; init;}
    public string Content {get; init;}
    public IDictionary<string, string> AdditionalHeaders {get; init;}
}

internal readonly struct WebhookRegistered
{
    public Guid EventId {get; init;}
    public DateTime CreatedAt {get; init;}
    public RegisterWebhookCommand Webhook {get; init;}
}
