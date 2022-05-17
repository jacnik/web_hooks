
# Requirements

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


# Auhorization
For demo purposes jwt token can be obtained from https://jwt.io/


# Ideas (failed and otherwise):
- Graph Ql is not the best tool for the job, make a rest api.
- Specially when returninng paged collections try to be a bit HAL compliant and add _links and _response objects.
- Add possibility to register for updates on specific symbols.
- Add possibility to register for updates on specific symbols and intervals [M1, M5, M15, M30, H1, H4, D1].
- How to call starting timestamp of a price object? start_timestamp, valid_from?
- Use Unix millis since epoch format for storing datetime.
- Use LiteDb for one file documentDb [https://www.litedb.org/docs/getting-started/]
- Also use LiteDb to create integration tests, be controversial and don't write unit tests.
- Here is some webhooks documentation: https://docs.microsoft.com/en-us/aspnet/webhooks/