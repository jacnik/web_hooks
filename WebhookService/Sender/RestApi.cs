namespace WebhookService.Sender;

internal static class WebhookSenderRestApi
{
    internal static async Task<IResult> PostActionEvent(
        ActionEvent action,
        WebhookScheduler scheduler,
        CancellationToken ct)
    {
        var schedule = await scheduler.OnActionHappened(action);
        return Results.Ok(schedule);
    }
}
