using LiteDB;

using WebhookService.Registration;

// TODO Logging

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

app.MapPost("/webhooks", WebhookRegistrationRestEndpoint.PostWebhook);
app.MapGet("/webhooks", WebhookRegistrationRestEndpoint.GetWebhooks);
app.MapGet("/webhooks/{id}", WebhookRegistrationRestEndpoint.GetWebhookById);

// TRIGGER sending webhooks, return summary

app.Run();
