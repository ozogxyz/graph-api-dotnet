// Copyright (c) Microsoft Corporation. All rights reserved.
// Licensed under the MIT license.

using Azure.Core;
using Azure.Identity;
using Microsoft.Graph;
using Microsoft.Graph.Models;
using Microsoft.Graph.DeviceManagement.DeviceConfigurations;

class GraphHelper
{
    // Settings object
    private static Settings? _settings;

    // User auth token credential
    private static ClientSecretCredential? _clientSecretCredential;

    // Client configured with app-only authentication
    private static GraphServiceClient? _appClient;

    public static void InitializeGraphForAppOnlyAuth(Settings settings)
    {
	_ = settings ??
	    throw new ArgumentNullException("Settings cannot be null.");
	_settings = settings;

	_clientSecretCredential ??= new ClientSecretCredential(
	    _settings.TenantId, _settings.ClientId, _settings.ClientSecret);
	
        _appClient ??= new GraphServiceClient(
	    _clientSecretCredential,
	    ["https://graph.microsoft.com/.default"]);
    }

    public static async Task<string> GetAppOnlyTokenAsync()
    {
	_ = _clientSecretCredential ??
	    throw new NullReferenceException("Graph not initialized for app-only token");
	var context = new TokenRequestContext(["https://graph.microsoft.com/.default"]);
	var response = await _clientSecretCredential.GetTokenAsync(context);
	return response.Token;
    }
    
    public static Task<UserCollectionResponse?> GetUserAsync()
    {
	_ = _appClient ??
	    throw new NullReferenceException("Failed to initialize Graph");

	return _appClient.Users.GetAsync((config) =>
	{
	    config.QueryParameters.Select = ["displayName", "id", "mail"];
	});
    }

    public static Task<DeviceCollectionResponse?> GetDeviceAsync()
    {
	_ = _appClient ??
	    throw new NullReferenceException("Failed to initialize Graph");

	// return _appClient.Devices.GetAsync((config) =>
	// {
	//     config.QueryParameters.Select = ["displayName", "id", "mail"];
	// });
	return _appClient.Devices.GetAsync();
    }

    public static Task<DeviceConfigurationCollectionResponse?> GetConfigsAsync()
    {
	_ = _appClient ??
	    throw new NullReferenceException("Failed to initialize Graph");

	return _appClient.DeviceManagement.DeviceConfigurations.GetAsync();
    }


    public static async Task MakeGraphCallAsync()
    {
	try
	{
	    // Get Users
	    var devicePage = await GraphHelper.GetDeviceAsync();
	    if (devicePage?.Value == null) { Console.WriteLine("No device found"); return; }
	    foreach (var device in devicePage.Value)
	    {
		Console.WriteLine($"Device: {device.DisplayName ?? "NO NAME"}");
		Console.WriteLine($"  ID: {device.Id}");
	    }

	    // Get Device Configurations
	    var configs = await GraphHelper.GetConfigsAsync();
	    if (configs?.Value == null) { Console.WriteLine("No config found"); return; }
	    foreach (var config in configs.Value)
	    {
		Console.WriteLine($"Config: {config.DisplayName ?? "NO NAME"}");
		Console.WriteLine($"  ID: {config.Id}");
	    }
	
	}
	catch (Exception ex)
	{
	    Console.WriteLine($"Error getting devices: {ex.Message}");
	}
	_ = _appClient ??
	    throw new NullReferenceException("Failed to initialize Graph.");
	await _appClient.DeviceManagement.DeviceConfigurations.GetAsync();
    }
}
