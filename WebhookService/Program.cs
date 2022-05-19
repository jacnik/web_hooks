using LiteDB;
using WebhookService.Registration;


var builder = WebApplication.CreateBuilder(args);

var currentDir = Directory.GetCurrentDirectory();
var dbName = "WebhookData.db";
var dbPath = Path.Join(currentDir, dbName);

builder.Services
    .AddSingleton<LiteDatabase>(new LiteDatabase(dbPath))
    .AddSingleton<IWebhookRegistrationRepository, WebhookRegistrationRepository>()
    .AddSingleton<ILiteCollection<WebhookRegistered>>(p =>
        p.GetRequiredService<LiteDatabase>().GetCollection<WebhookRegistered>("WebhookRegistrations"));



var app = builder.Build();

app.UseHttpsRedirection();

app.MapGet("/", () => "Root");

app.MapPost("/webhooks", WebhookRegistrationRestHandlers.PostWebhook);
app.MapGet("/webhooks", WebhookRegistrationRestHandlers.GetWebhooks);
app.MapGet("/webhooks/{id}", WebhookRegistrationRestHandlers.GetWebhookById);

// TRIGGER sending webhooks, return summary

app.Run();
