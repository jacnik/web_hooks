namespace WebhookService.Registration;


internal static class WebhookRegistrationRestApi
{
    internal static async Task<IResult> PostWebhook(RegisterWebhookCommand cmd, IWebhookRegistrationRepository db, CancellationToken ct)
    {
        var webhookRegistered = new WebhookRegistered
            {
                CreatedAt = DateTime.UtcNow,
                EventId = Guid.NewGuid(),
                Webhook = cmd
            };
        var dbRsp = await db.Insert(webhookRegistered, ct);

        return dbRsp.Result.Match(
            success => Results.Created($"/webhooks/{success.Value.EventId}", success.Value),
            notFound => Results.NotFound(),
            error => Results.StatusCode(StatusCodes.Status500InternalServerError)
        );
    }
    internal static async Task<IResult> GetWebhooks(IWebhookRegistrationRepository db, CancellationToken ct)
    {
        var dbRsp = await db.GetAll(ct);
        return dbRsp.Result.Match(
            success => Results.Ok(success.Value),
            notFound => Results.NotFound(),
            error => Results.StatusCode(StatusCodes.Status500InternalServerError)
        );
    }

    internal static async Task<IResult> GetWebhookById(Guid id, IWebhookRegistrationRepository db, CancellationToken ct)
    {
        var dbRsp = await db.GetById(id, ct);
        return dbRsp.Result.Match(
            success => Results.Ok(success.Value),
            notFound => Results.NotFound(),
            error => Results.StatusCode(StatusCodes.Status500InternalServerError)
        );
    }
}