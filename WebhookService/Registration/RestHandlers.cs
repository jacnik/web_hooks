namespace WebhookService.Registration;


internal static class WebhookRegistrationRestHandlers
{
    internal static async Task<IResult> PostWebhook(RegisterWebhookCommand cmd, IWebhookRegistrationRepository db, CancellationToken ct)
    {
        var webhookRegistered = new WebhookRegistered
            {
                CreatedAt = DateTime.UtcNow,
                EventId = Guid.NewGuid(),
                Webhook = cmd
            };
        await db.Insert(webhookRegistered, ct);

        return Results.Created($"/webhooks/{webhookRegistered.EventId}", webhookRegistered);
    }
    internal static async Task<IResult> GetWebhooks(IWebhookRegistrationRepository db, CancellationToken ct) => Results.Ok(await db.GetAll(ct));
    internal static async Task<IResult> GetWebhookById(Guid id, IWebhookRegistrationRepository db, CancellationToken ct)
    {
        return await db.GetById(id, ct) is WebhookRegistered webhook
            ? Results.Ok(webhook)
            : Results.NotFound();
    }
}