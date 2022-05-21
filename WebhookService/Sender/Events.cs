namespace WebhookService.Sender;


internal struct ActionEvent
{
    /**
    This is supposed to simulate a event happened in system.
    It should trigger webhook sending.
    */
    public string EventName {get; init;}
}
internal class WebhooksScheduled
{
    public Guid ProcessId {get; init;}
    public ActionEvent Action {get; init;}
}
