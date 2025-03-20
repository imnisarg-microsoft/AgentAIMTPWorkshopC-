// See https://aka.ms/new-console-template for more information
using AutogenDotNet.Agents;
using AutogenDotNet.Connectors;
using Azure.AI.OpenAI;
using Azure.Core;
using Azure.Identity;
using Microsoft.Identity.Client;
using Microsoft.TeamFoundation.Core.WebApi;
using Microsoft.TeamFoundation.SourceControl.WebApi;
using Microsoft.VisualStudio.Services.Client;
using Microsoft.VisualStudio.Services.WebApi;
using System.Net.Http.Headers;


//await AssistantAgentDemo.RunAsync();

TokenCredential managedIdentityCredential = new DefaultAzureCredential();

var modelId = "gpt-4o";
var endpoint = new Uri("https://dex-jpn-openai-stg.openai.azure.com/");
var openAIClient = new AzureOpenAIClient(endpoint, managedIdentityCredential);
HttpClient httpClient = new();

VssAadToken vssAadToken = new(scopes =>
{
    AccessToken tokenResult = managedIdentityCredential.GetTokenAsync(new TokenRequestContext(VssAadSettings.DefaultScopes), CancellationToken.None).Result;
    return new AuthenticationResult(tokenResult.Token, false, string.Empty, tokenResult.ExpiresOn, tokenResult.ExpiresOn, string.Empty, null, null, scopes, Guid.Empty);
});

VssAadCredential vssAadCredential = new(vssAadToken);

VssClientHttpRequestSettings vssClientHttpRequestSettings = VssClientHttpRequestSettings.Default.Clone();
vssClientHttpRequestSettings.UserAgent =
    [
    new ProductInfoHeaderValue("Demo","1.0"),
];

Uri organizationUri = new(new Uri(uriString: "https://dev.azure.com"), "microsoft");
VssConnection vssConnection = new VssConnection(organizationUri, vssAadCredential, vssClientHttpRequestSettings);

ProjectHttpClient projectHttpClient = vssConnection.GetClient<ProjectHttpClient>();
GitHttpClient gitHttpClient = vssConnection.GetClient<GitHttpClient>();


var adoService = new ADORepositoryService(projectHttpClient, gitHttpClient, httpClient);
var repos = await adoService.IterateOverRepositoriesAsync("Windows Defender");
foreach (GitRepository repo in repos)
{
    Console.WriteLine("Repo ID, Name and URL : {0} {1} {2}", repo.Id, repo.Name, repo.RemoteUrl);
}
var file = await adoService.GetFileAsString("NotebookDeploymentController.cs", "Windows Defender", "MD.Common.Services.NotebookServices");
Console.WriteLine(file);

//await new OpenAIChatAgentDemo(openAIClient, modelId).RunAsync();
//await new OpenAIAgentWithUserProxy(openAIClient, modelId).RunAsync();
//await new AgentWithFunctionCall(openAIClient, modelId).RunAsync();
//await new MiddlewareAgentDemo(openAIClient, modelId).RunAsync();