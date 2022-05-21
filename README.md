
# Usage
Example requests are in `requests.http` file.
In vscode they can be send directly with [REST client](https://marketplace.visualstudio.com/items?itemName=humao.rest-client)
Or by using curl after expanding variables.


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


# Notes
- Since LiteDb doesn't support async opeations Tasks where used as a way to simulate it.


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

***

# Ideas (failed or otherwise):
- Graph Ql is not the best tool for the job, make a rest api.
- Specially when returninng paged collections try to be a bit HAL compliant and add _links and _response objects.
- Add possibility to register for updates on specific symbols.
- Add possibility to register for updates on specific symbols and intervals [M1, M5, M15, M30, H1, H4, D1].
- How to call starting timestamp of a price object?
    - start_timestamp?
    - valid_from?
- Use Unix millis since epoch format for storing datetime.
- Use LiteDb for one file documentDb [https://www.litedb.org/docs/getting-started/]
    - Unfortunatel it is synchronous so wrap calls to it's methods in Task to mimick async/await.
- Also use LiteDb to create integration tests, be controversial and don't write unit tests.
- Here is some webhooks documentation:
    - [https://docs.microsoft.com/en-us/aspnet/webhooks/]
    - [https://devblogs.microsoft.com/dotnet/sending-webhooks-with-asp-net-webhooks-preview/] -> this one talks about Azure Table Storage which is obsolete.
    - Looks like all the nugets for webhooks are at least 2 years old and tareting net45.
- Try only adding events to db, no updates and deletes. At least add soft-delete.
- Separate the process of calling all webhook endpoint from event triggerging this process, so it returns immediatly after trigger.
    - When using bakground task for calling webhook urls with in memory queue not all webhooks will be called when service will be redeployed during this process.
    - Pass cancellation token to this process on shutdown to handle it gracefully.
- Url + trigger might be the id for each webhook, but how to handle permission to delete a webhook?
    - add a owner property?
    - have a separate subscribers collection?
- Add a async exponential backoff for failed webhooks.
- Have a threshold for number of failed webhooks call after which it gets disabled.
- This would be a nice example of event sourcing and cqrs, but not without a mq, see [Requirements](###what-not-to-do).
- When a price update happens all webhooks registered for that event with symbol and interval will be called.
    - There will be no per user events.
    - Although there could be, per user or even per group of users, like in one organization, or from 'premium tier'
- Load tests wouldn't go amiss, after all performance is a feature, degrading it is a bug. NBomber.
- Can add cache for webhooks [https://docs.microsoft.com/en-us/aspnet/core/performance/caching/memory?view=aspnetcore-6.0].

- Split project by features/slices.
- appsettings.json file would need to be added so the service can be deployed to different stages.


