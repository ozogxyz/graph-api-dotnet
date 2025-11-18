using Azure.Core;
using Azure.Identity;
using Microsoft.Graph;
using Microsoft.Graph.Models;

public class GraphHelper
{
    private static Settings? _settings;
    private static ClientSecretCredential? _credential;
    private static GraphServiceClient? _graphClient;
    private static readonly string[] _graphScopes = new[] { "https://graph.microsoft.com/.default" };

    public static void InitializeGraph(Settings settings)
    {
        _settings = settings ?? throw new ArgumentNullException(nameof(settings));

        _credential ??= new ClientSecretCredential(
            _settings.TenantId,
	    _settings.ClientId,
	    _settings.ClientSecret
	);

        _graphClient ??= new GraphServiceClient(
	    _credential,
	    new[] { "https://graph.microsoft.com/.default" }
	    );
    }

    public static async Task<string> GetTokenAsync()
    {
        _ = _credential ??
            throw new NullReferenceException("Graph not initialized");
        var context = new TokenRequestContext(["https://graph.microsoft.com/.default"]);
        var response = await _credential.GetTokenAsync(context);
        return response.Token;
    }

    public static Task<UserCollectionResponse?> GetUserAsync() =>
	_graphClient!.Users.GetAsync((config) =>
        {
            config.QueryParameters.Select = ["displayName", "id", "mail"];
        });

    public static Task<DeviceCollectionResponse?>
	GetDeviceAsync() =>
	_graphClient!.Devices.GetAsync();

    public static Task<DeviceConfigurationCollectionResponse?>
	GetDeviceConfigsAsync() =>
	_graphClient!.DeviceManagement.DeviceConfigurations.GetAsync();

    public static Task<DeviceConfigurationDeviceStatusCollectionResponse?>
	GetDeviceConfigStatusesAsync(string configId) =>
	_graphClient!.DeviceManagement.DeviceConfigurations[configId].DeviceStatuses.GetAsync();

    public static Task<DeviceConfigurationAssignmentCollectionResponse?>
	GetConfigAssignmentsAsync(string configId) =>
	_graphClient!.DeviceManagement.DeviceConfigurations[configId].Assignments.GetAsync();
    
    private static async Task<HttpResponseMessage>
	SendRawGraphRequestAsync(string url)
    {
	_ = _credential ?? throw new NullReferenceException("Graph not initialized.");
	var token = await _credential.GetTokenAsync(
	    new Azure.Core.TokenRequestContext(_graphScopes));

	using var http = new HttpClient();
	var request = new HttpRequestMessage(HttpMethod.Get, url);
	request.Headers.Authorization = new System.Net.Http.Headers.AuthenticationHeaderValue("Bearer", token.Token);
	return await http.SendAsync(request);
    }

    public static async Task<string> GetSettingsCatalogPoliciesRawAsync()
    {
	var url = "https://graph.microsoft.com/beta/deviceManagement/configurationPolicies";
	var response = await SendRawGraphRequestAsync(url);
	response.EnsureSuccessStatusCode();
	return await response.Content.ReadAsStringAsync();
    }

}
