using Xunit;
using Microsoft.AspNetCore.Mvc.Testing;

namespace WebhookService.Tests;

public class TriggeringWebhooksShould
{
    public TriggeringWebhooksShould()
    {
        // Arrange
        // _server = new TestServer(new WebHostBuilder()
        //    .UseStartup<Startup>());
        // _client = _server.CreateClient();
    }

    [Fact]
    public void Test1()
    {
        var application = new WebApplicationFactory<Program>()
        .WithWebHostBuilder(builder =>
        {
            // ... Configure test services
        });

        var client = application.CreateClient();
        Assert.False(1 == 2, "1 should not be prime");
    }
}