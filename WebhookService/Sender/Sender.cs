using System.Threading.Channels;


namespace WebhookService.Sender;


internal class WebhookSender : BackgroundService
{
    private readonly ChannelReader<WebhooksScheduled> schedules;
    private readonly IWebhookSenderRepository repo;
    private readonly ILogger<WebhookSender> logger;


    public WebhookSender(
        ChannelReader<WebhooksScheduled> schedules,
        IWebhookSenderRepository repo,
        ILogger<WebhookSender> logger)
    {
        this.schedules = schedules;
        this.repo = repo;
        this.logger = logger;
    }

    public async Task OnSchedule(WebhooksScheduled schedule, HttpClient client, CancellationToken ct)
    {
        var triggersRsp = await repo.GetForTrigger(schedule.Action.EventName);
        await triggersRsp.Result.Match(
            async success => {
                if (logger.IsEnabled(LogLevel.Information))
                {
                    logger.LogInformation("Got registered webhooks.");
                }
                foreach (var webhook in success.Value)
                {
                    var httpRsp = await client.PostAsync(
                        webhook.Webhook.Url,
                        new StringContent(webhook.Webhook.Content),
                        ct);
                }
                if (logger.IsEnabled(LogLevel.Information))
                {
                    logger.LogInformation("Send webhooks for schedule {0}.", schedule.ToString());
                }
            },
            error => Task.Run(() => {
                if (logger.IsEnabled(LogLevel.Error))
                {
                    logger.LogError("Failed to get registered webhooks. {0}", error.Reason.Message);
                }
            })
        );
    }

    protected override async Task ExecuteAsync(CancellationToken ct)
    {
        using var client = new HttpClient(); // TOOD inject interface
        while (await this.schedules.WaitToReadAsync(ct))
        {
            var schedule = await this.schedules.ReadAsync(ct);
            await OnSchedule(schedule, client, ct);
        }
    }

    public override async Task StopAsync(CancellationToken ct)
    {
        if (logger.IsEnabled(LogLevel.Information))
        {
            logger.LogInformation("WebhookSender Hosted Service is stopping.");
        }
        await base.StopAsync(ct);
    }
}
