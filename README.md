
# Usage
### Start service
```sh
cd ./WebhookService
dotnet run
```
### Use service
> Example requests are in `requests.http` file.
In vscode they can be send directly with [REST client](https://marketplace.visualstudio.com/items?itemName=humao.rest-client). Or by using curl after expanding variables.
#### Basic usage
```sh
# Register Webhook
curl --request POST \
  --url 'http://localhost:5000/webhooks' \
  --header 'authorization: Bearer eyJhbGciOiJIUzI1NiIsInR5cCI6IkpXVCJ9.eyJzdWIiOiIxMjM0NTY3ODkwIiwibmFtZSI6IkpvaG4gRG9lIiwiaWF0IjoxNTE2MjM5MDIyfQ.cThIIoDvwdueQB468K5xDc5633seEFoqwxjF_xSJyQQ' \
  --header 'content-type: application/json' \
  --data '{
    "url": "https://en9bayo995ajl.x.pipedream.net",
    "trigger": "something_happened",
    "content": "123.456.789",
    "additionalHeaders": {}}'

# trigger sending webhooks
curl --request POST \
  --url 'http://localhost:5000/events' \
  --header 'authorization: Bearer eyJhbGciOiJIUzI1NiIsInR5cCI6IkpXVCJ9.eyJzdWIiOiIxMjM0NTY3ODkwIiwibmFtZSI6IkpvaG4gRG9lIiwiaWF0IjoxNTE2MjM5MDIyfQ.cThIIoDvwdueQB468K5xDc5633seEFoqwxjF_xSJyQQ' \
  --header 'content-type: application/json' \
  --data '{"eventName": "something_happened"}'
```


# Improvements
- logging
- Rate limit outbound request to same domain to protect agaist being part of an involuntary DDoS
- Create 'smart' Guid types, ex WebhookRegisteredId
- Avoid primitive obsession by adding value objects or Enums
- Validate value objects during deserialization.
- Replace naming conventions (appdending ..Command to class names) by creating interfaces, even empty ICommand?
- Add swagger
- Add authentication/authorization
- Instead of making RbResponse types generic it would be better if each method defined it's own result type
- Catch specific Exceptions, not base Exception
- Partition webhook sending to separate Background tasks/Threads
- Figure out authentication for sending webhooks
- Constraint on registring two webhooks for the same trigger and url
- Page responses.
- WebhookSender should be in a pool so work can be offloaded on different instances.
- check for response status when sending webhook. Add async exponential backoff on failure.
- save and expose info about webhooks send so far with process id so used can query for it.
- Have a threshold for number of failed webhooks call after which it gets disabled.
- This would be a nice example of event sourcing and cqrs, but not without a mq.


# Notes
- Since LiteDb doesn't support async opeations Tasks where used as a way to simulate it.
- Urls from https://docs.webhook.site/ returned 404 so used [requestbin](https://requestbin.com/r/enpo0mqeo4e/29U14352yiYct4GUgv4roYRW1Pw) instead



# Excercise

This exercise will be used to discuss a real but tiny project and to check that we have a common understanding of how a web project should be developed.
If time is an issue (for some people a takehome exercise is not possible with family/other commitments, we can discuss alternatives).

We'd like you to create a simple ASP.NET Core (.Net 6) web application.

## The scenario:
you have some business application that end users or clients would like to integrate with.
You have already built an API, but the clients would like to be notified when something happens in your system, so that their applications can react and process the update.
You therefore decide to build a small standalone application that can handle these updates and send out webhooks

### Requirements:

    Asp.Net Core web application that can send webhook requests out
    User can add a "webhook endpoint"
    API endpoint or button that when hit, sends a POST request to all webhook endpoints currently registered

### Hints:

    Use something like https://docs.webhook.site/ to test the webooks, without building a test endpoint

## The following items we consider out of scope for this exercise, as otherwise it could take too long. We however would expect you to be able to explain at a high level how these points below could be achieved.

### What NOT to do:

    Seperate backend and frontend - while this might be a good practise (and what we do in real life), this can add significant time to the exercise. We would however discuss how you would implement it and handle the communication between frontend and backend.
    Authentication (unless you want to, and start with a dotnet template that includes it already built in)
    Full SQL database: it'll be easier for us if you use sqlite.
    Scaling, "webscale", etc - however we'd like to discuss how you would scale it if you needed
    Queuing approach/message bus

### The goal of this exercise is:

    We should be able to open the solution
    Add/Register a webhook endpoint (via an API request (ApiController) or simple UI (UI can be a simple razor page or a ASP.NET MVC app or a Vue.js app or ...))
    Trigger the sending of webhooks (via a button or API call)
    Discuss the code and approaches

