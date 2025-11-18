// Copyright (c) Microsoft Corporation. All rights reserved.
// Licensed under the MIT license.

using Azure.Core;
using Azure.Identity;
using Microsoft.Graph;
using Microsoft.Graph.Models;

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

    #pragma warning disable CS1998
    public static async Task MakeGraphCallAsync() {}
}
