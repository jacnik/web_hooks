using LiteDB;
using WebhookService.Registration;
using WebhookService.Sender;

var builder = WebApplication.CreateBuilder(args);

var currentDir = Directory.GetCurrentDirectory();
var dbName = "WebhookData.db";
var dbPath = Path.Join(currentDir, dbName);

builder.Services
    .AddSingleton<LiteDatabase>(new LiteDatabase(dbPath))
    .AddSingleton<IWebhookRegistrationRepository, WebhookRegistrationRepository>()
    .AddSingleton<ILiteCollection<WebhookRegistered>>(p =>
        p.GetRequiredService<LiteDatabase>().GetCollection<WebhookRegistered>("WebhookRegistrations"))
    .AddSingleton<IWebhookSenderRepository, WebhookSenderRepository>()
    .AddSingleton<WebhookSender>();


var app = builder.Build();

app.UseHttpsRedirection();


app.MapGet("/", () => "Root");

app.MapPost("/webhooks", WebhookRegistrationRestApi.PostWebhook);
app.MapGet("/webhooks", WebhookRegistrationRestApi.GetWebhooks);
app.MapGet("/webhooks/{id}", WebhookRegistrationRestApi.GetWebhookById);

app.MapPost("/events", WebhookSenderRestApi.PostActionEvent);

app.Run();
