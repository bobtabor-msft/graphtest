using Azure.Identity;
using System.Net.Http.Headers;

namespace graphtest;

public class Program
{
    public static async Task Main(string[] args)
    {
        try
        {
            await AuthenticateAndRunTestsAsync();
        }
        catch (Exception ex)
        {
            Console.WriteLine($"❌ An error occurred: {ex.Message}");
        }
    }

    private static async Task AuthenticateAndRunTestsAsync()
    {
        // Use Azure Identity to authenticate
        // Replace with your own tenant and client ID
        var tenantId = "";
        var clientId = "";

        var credential = new InteractiveBrowserCredential(new InteractiveBrowserCredentialOptions
        {
            TenantId = tenantId,
            ClientId = clientId
        });

        var tokenRequestContext = new Azure.Core.TokenRequestContext(
        new[] { "https://graph.microsoft.com/.default" });

        var token = await credential.GetTokenAsync(tokenRequestContext);

        Console.WriteLine("✅ Token acquired.");

        using var client = new HttpClient();
        client.DefaultRequestHeaders.Authorization = new AuthenticationHeaderValue("Bearer", token.Token);

        await Manager.ExecuteFirstTest(client);
        Console.WriteLine("🔄 First test completed.");

        // Execute the second test
        await Manager.ExecuteSecondTest(client);
        Console.WriteLine("🔄 Second test completed.");

        await Manager.ExecuteThirdTest(client);
        Console.WriteLine("🔄 Third test completed.");
    }
}

