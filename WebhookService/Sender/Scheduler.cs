using System.Threading.Channels;

namespace WebhookService.Sender;

internal class WebhookScheduler
{
    private readonly ChannelWriter<WebhooksScheduled> schedules;
    public WebhookScheduler(ChannelWriter<WebhooksScheduled> schedules)
    {
        this.schedules = schedules;
    }

    public async Task<WebhooksScheduled> OnActionHappened(ActionEvent action)
    {
        var schedule = new WebhooksScheduled
        {
            ProcessId = Guid.NewGuid(),
            Action = action
        };
        await schedules.WriteAsync(schedule);
        return schedule;
    }
}