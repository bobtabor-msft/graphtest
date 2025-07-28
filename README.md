# Graph API test

To use this, you'll need to first Set up Entra ID, then update the code with your client and tenant IDs.

## Step 1: Set up Entra ID

1. Azure portal
2. Microsoft Entra ID
3. Add > App registration
4. Register an application
  - Name
  - Supported account types: "Accounts in this organization directory only"
  - Redirect URL: "Public client/native (mobile & desktop)" - http://localhost

Find it in Manage > App registrations 

5. Manage > API permissions ... Add a permission
  - In the "Request API permissions", under "Microsoft APIs", select "Microsoft Graph"
  - Choose "Delegated permissions"
  - Under "Select permissions" filter using term "copilot"

There are four permissions listed here.

**CopilotSettings-LimitedMode**

- CopilotSettings-LimitedMode.Read

  Read organization-wide copilot limited mode setting
  
  Admin consent required: yes

- CopilotSettings-LimitedMode.ReadWrite

  Read and write organization-wide copilot limited mode setting

  Admin consent required: yes

**SecurityCopilotWorkspaces**

- SecurityCopilotWorkspaces.Read.All

  Read all Security Copilot resources for the signed-in user

  Admin consent required: no

- SeucirtyCOpilotWorkspaces.ReadWrite.All

  Read and write individually owned Secuiryt Copilot resources of the signed-in user

  Admin consent required: no

![Screenshot of the Request API permissions dialog](/screenshot.png)


Back in the overview, you will need the "Application (client) ID" and the "Directory (tenant) ID".

## Step 2: Update client and tenant IDs

In the `Program.cs`, on lines 24 and 25, replace those values with that you retrieved from the previous step.


## Step 3: Run the app

```bash
dotnet run
```

Three tests from the `Manager.cs` will run.

### Test 1

Tests access to https://graph.microsoft.com/v1.0/copilot ... This helps determine if Copilot APIs are available in your tenant. The first test is likely to succeed!

```
✅ Token acquired.
👋 Hello, Bob Tabor (rotabor_microsoft.com#EXT#@MicrosoftFieldLedSandbox.onmicrosoft.com)
🔄 First test completed.
```

### Test 2

Attempts to access https://graph.microsoft.com/v1.0/copilot/users/me/interactionHistory/getAllEnterpriseInteractions ... This is one of the actual Copilot APIs that retrieves user's interactions with Copilot. Requires AiEnterpriseInteraction.Read.All permission (typically application permission). The second test requires licenses / permissions.

```output
❌ Error fetching files:
{"error":{"code":"BadRequest","message":"Tenant does not have a SPO license.","innerError":{"date":"2025-07-28T15:18:16","request-id":"eea299da-0978-44e1-83d2-b26aff69a820","client-request-id":"eea299da-0978-44e1-83d2-b26aff69a820"}}}

❌ Error fetching Teams:
{"error":{"code":"Unauthorized","message":"UnknownError","innerError":{"code":"Unauthorized","date":"2025-07-28T15:18:17","request-id":"f6b19edc-8cc3-4c5a-8877-cce01759c62f","client-request-id":"f6b19edc-8cc3-4c5a-8877-cce01759c62f"}}}
🔄 Second test completed.
```

### Test 3

The third test is where we try to use the M365 **Copilot** API. Tests the beta version of Copilot APIs at https://graph.microsoft.com/beta/copilot ... Some newer Copilot features might be available in beta.

```output
🤖 Testing Microsoft 365 Copilot API Access...
✅ Copilot base endpoint accessible:
{"admin@navigationLink":"https://graph.microsoft.com/v1.0/copilot/admin","interactionHistory@navigationLink":"https://graph.microsoft.com/v1.0/copilot/interactionHistory","users@navigationLink":"https://graph.microsoft.com/v1.0/copilot/users"}

� Attempting to access Copilot interaction history...
Current user info: {"@odata.context":"https://graph.microsoft.com/v1.0/$metadata#users(id)/$entity","id":"592c56ba-d954-4787-bf8a-3a1a65da904c"}
❌ Copilot history not accessible: PreconditionFailed
Details: {"error":{"code":"PreconditionFailed","message":"Requested API is not supported in delegated context","innerError":{"date":"2025-07-28T15:18:18","request-id":"2063e456-aa15-4639-bd0f-fcbce90932bb","client-request-id":"2063e456-aa15-4639-bd0f-fcbce90932bb"}}}

🔬 Testing beta Copilot endpoints...
✅ Beta Copilot endpoint accessible:
{"admin@navigationLink":"https://graph.microsoft.com/beta/copilot/admin","settings@navigationLink":"https://graph.microsoft.com/beta/copilot/settings","interactionHistory@navigationLink":"https://graph.microsoft.com/beta/copilot/interactionHistory","users@navigationLink":"https://graph.microsoft.com/beta/copilot/users","#microsoft.graph.retrieval": {}}
```

### Important Notes:

**Licensing Requirements:**

Microsoft 365 Copilot APIs require a Microsoft 365 Copilot license for each user
This is different from regular Microsoft Graph APIs

**Permission Requirements:**

- Most Copilot APIs require Application permissions (not delegated)
- Common permissions needed: AiEnterpriseInteraction.Read.All, AiInteractionHistory.Read.All
- These typically require admin consent
- Expected Behavior with Minimal Permissions:

  - You'll likely see 403 (Forbidden) or 404 (Not Found) responses
  - This is expected if you don't have Copilot licenses or the required permissions
  - The code handles these gracefully and explains what's needed
  - This approach allows you to test actual Copilot endpoints while understanding why they might not be accessible with minimal permissions. The implementation provides educational value by showing what permissions and licenses would be required for full access.


