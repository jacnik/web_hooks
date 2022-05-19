using WebhookService.Registration;

internal interface IWebhookSenderRepository
{
    public WebhookRegistered /*TODO composite return type */ GetForTrigger(string trigger);

}

internal class WebhookSender
{

}