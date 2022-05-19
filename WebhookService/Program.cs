using LiteDB;

using WebhookService.Registration;


var builder = WebApplication.CreateBuilder(args);

var currentDir = Directory.GetCurrentDirectory();
var dbName = "WebhookData.db";
var dbPath = Path.Join(currentDir, dbName);

builder.Services
    .AddSingleton<LiteDatabase>(new LiteDatabase(dbPath))
    .AddSingleton<IWebhookRegistrationRepository, WebhookRegistrationRepository>();


var app = builder.Build();

app.UseHttpsRedirection();

app.MapGet("/", () => "Root");
app.MapPost("/webhooks", async (RegisterWebhookCommand cmd, IWebhookRegistrationRepository db, CancellationToken cancellationToken) =>
{
    var webhookRegistered = new WebhookRegistered
    {
        CreatedAt = DateTime.UtcNow,
        EventId = Guid.NewGuid(),
        Webhook = cmd
    };
    await db.Insert(webhookRegistered);

    return Results.Created($"/webhooks/{webhookRegistered.EventId}", webhookRegistered);
});

app.MapGet("/webhooks", async (IWebhookRegistrationRepository db, CancellationToken cancellationToken) =>
    await db.GetAll()
);

app.MapGet("/webhooks/{id}", async (Guid id, IWebhookRegistrationRepository db, CancellationToken cancellationToken) =>
    await db.GetById(id) is WebhookRegistered webhook
        ? Results.Ok(webhook)
        : Results.NotFound()
);

// TRIGGER sending webhooks, return summary

app.Run();
