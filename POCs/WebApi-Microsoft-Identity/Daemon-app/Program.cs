using Microsoft.Identity.Client;
using System;
using System.Net.Http;
using System.Net.Http.Headers;

HttpClient client = new HttpClient();

var clientId = "<your-daemon-app-client-id>";
var clientSecret = "<your-daemon-app-secret>";
var scopes = new[] { "api://<your-web-api-application-id>/.default" };
var tenantName = "<your-tenant-name>";
var tenantId = "<your_tenant_id>";
var authority = $"https://{tenantName}.ciamlogin.com/{tenantId}";

var app = ConfidentialClientApplicationBuilder
    .Create(clientId)
    .WithAuthority(authority)
    .WithClientSecret(clientSecret)
    .Build();

var result = await app.AcquireTokenForClient(scopes).ExecuteAsync();

client.DefaultRequestHeaders.Authorization = new AuthenticationHeaderValue("Bearer", result.AccessToken);
var response = await client.GetAsync("https://localhost:7142/api/todolist");
Console.WriteLine("Your response is: " + response.StatusCode);