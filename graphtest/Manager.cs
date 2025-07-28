using System.Net.Http.Json;

namespace graphtest;

public class Manager
{
    public static async Task ExecuteFirstTest(HttpClient client)
    {
        // Call Graph /me endpoint
        var response = await client.GetAsync("https://graph.microsoft.com/v1.0/me");

        if (response.IsSuccessStatusCode)
        {
            var me = await response.Content.ReadFromJsonAsync<User>();
            Console.WriteLine($"👋 Hello, {me?.DisplayName} ({me?.UserPrincipalName})");
        }
        else
        {
            Console.WriteLine("❌ Error calling /me:");
            Console.WriteLine(await response.Content.ReadAsStringAsync());
        }
    }

    public static async Task ExecuteSecondTest(HttpClient client)
    {
        // ✅ Option A: Get your recent OneDrive files
        var filesResponse = await client.GetAsync("https://graph.microsoft.com/v1.0/me/drive/recent");

        if (filesResponse.IsSuccessStatusCode)
        {
            var json = await filesResponse.Content.ReadAsStringAsync();
            Console.WriteLine("📁 Recent OneDrive/SharePoint Files:");
            Console.WriteLine(json);
        }
        else
        {
            Console.WriteLine("❌ Error fetching files:");
            Console.WriteLine(await filesResponse.Content.ReadAsStringAsync());
        }

        // ✅ Option B: List Teams you're a member of
        var teamsResponse = await client.GetAsync("https://graph.microsoft.com/v1.0/me/joinedTeams");

        if (teamsResponse.IsSuccessStatusCode)
        {
            var json = await teamsResponse.Content.ReadAsStringAsync();
            Console.WriteLine("\n🧑‍🤝‍🧑 Teams You're In:");
            Console.WriteLine(json);
        }
        else
        {
            Console.WriteLine("\n❌ Error fetching Teams:");
            Console.WriteLine(await teamsResponse.Content.ReadAsStringAsync());
        }
    }
    
    public static async Task ExecuteThirdTest(HttpClient client)
    {
        try
        {
            // Try to check if user has access to any Copilot capabilities
            // First, let's try a simple approach - checking if the /copilot endpoint is accessible
            Console.WriteLine("\n🤖 Testing Microsoft 365 Copilot API Access...");
            
            // Test 1: Try to access the copilot endpoint structure (this might fail but gives us info)
            var copilotBaseResponse = await client.GetAsync("https://graph.microsoft.com/v1.0/copilot");
            
            if (copilotBaseResponse.IsSuccessStatusCode)
            {
                var json = await copilotBaseResponse.Content.ReadAsStringAsync();
                Console.WriteLine("✅ Copilot base endpoint accessible:");
                Console.WriteLine(json);
            }
            else
            {
                Console.WriteLine($"⚠️ Copilot base endpoint status: {copilotBaseResponse.StatusCode}");
                if (copilotBaseResponse.StatusCode == System.Net.HttpStatusCode.NotFound)
                {
                    Console.WriteLine("This might be expected - the base endpoint may not be directly accessible.");
                }
                else
                {
                    var errorContent = await copilotBaseResponse.Content.ReadAsStringAsync();
                    Console.WriteLine($"Error details: {errorContent}");
                }
            }

            // Test 2: Try accessing user-specific copilot interaction history (requires specific permissions)
            Console.WriteLine("\n� Attempting to access Copilot interaction history...");
            
            // First, get the current user's ID
            var meResponse = await client.GetAsync("https://graph.microsoft.com/v1.0/me?$select=id");
            if (meResponse.IsSuccessStatusCode)
            {
                var meData = await meResponse.Content.ReadAsStringAsync();
                Console.WriteLine($"Current user info: {meData}");
                
                // Extract user ID from response (simple approach)
                // In a real app, you'd parse JSON properly
                if (meData.Contains("\"id\":"))
                {
                    // Try the Copilot interaction history endpoint
                    // Note: This typically requires AiEnterpriseInteraction.Read.All permission
                    var historyResponse = await client.GetAsync("https://graph.microsoft.com/v1.0/copilot/users/me/interactionHistory/getAllEnterpriseInteractions?$top=1");
                    
                    if (historyResponse.IsSuccessStatusCode)
                    {
                        var historyJson = await historyResponse.Content.ReadAsStringAsync();
                        Console.WriteLine("✅ Copilot interaction history accessible:");
                        Console.WriteLine(historyJson);
                    }
                    else
                    {
                        Console.WriteLine($"❌ Copilot history not accessible: {historyResponse.StatusCode}");
                        var historyError = await historyResponse.Content.ReadAsStringAsync();
                        Console.WriteLine($"Details: {historyError}");
                    }
                }
            }

            // Test 3: Try the beta endpoint for any additional capabilities
            Console.WriteLine("\n🔬 Testing beta Copilot endpoints...");
            var betaCopilotResponse = await client.GetAsync("https://graph.microsoft.com/beta/copilot");
            
            if (betaCopilotResponse.IsSuccessStatusCode)
            {
                var betaJson = await betaCopilotResponse.Content.ReadAsStringAsync();
                Console.WriteLine("✅ Beta Copilot endpoint accessible:");
                Console.WriteLine(betaJson);
            }
            else
            {
                Console.WriteLine($"⚠️ Beta Copilot endpoint status: {betaCopilotResponse.StatusCode}");
                var betaError = await betaCopilotResponse.Content.ReadAsStringAsync();
                Console.WriteLine($"Details: {betaError}");
            }

            Console.WriteLine("\n📋 Summary:");
            Console.WriteLine("Microsoft 365 Copilot APIs typically require:");
            Console.WriteLine("1. Microsoft 365 Copilot license for the user");
            Console.WriteLine("2. Specific permissions like AiEnterpriseInteraction.Read.All (application permission)");
            Console.WriteLine("3. Admin consent for organizational access");
            Console.WriteLine("\nFor minimal permissions, regular Microsoft Graph APIs (like in Test 1 & 2) are more accessible.");

        }
        catch (HttpRequestException ex)
        {
            Console.WriteLine($"\n❌ HTTP Error: {ex.Message}");
        }
        catch (Exception ex)
        {
            Console.WriteLine($"\n❌ Unexpected Error: {ex.Message}");
        }
    }

    record User(string? DisplayName, string? UserPrincipalName);
}
