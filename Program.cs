Console.WriteLine(".NET Graph App\n");

var settings = Settings.LoadSettings();

GraphHelper.InitializeGraph(settings);

int choice = -1;

while (choice != 0)
{
    Console.WriteLine("\nMenu:");
    Console.WriteLine("0. Exit");
    Console.WriteLine("1. List Users");
    Console.WriteLine("2. List Devices");
    Console.WriteLine("3. List Device Configurations");
    Console.WriteLine("4. List Device Configuration Statuses");
    Console.WriteLine("5. List Settings Catalog Configurations for non-admin devices");

    try
    {
	choice = int.Parse(Console.ReadLine() ?? string.Empty);
    }
    catch (System.FormatException)
    {
	choice = -1;
    }

    switch (choice)
    {
	case 0:
	    Console.WriteLine("Exiting...");
	    break;

	case 1:
	    Console.WriteLine("\nListing users...");
	    var users = await GraphHelper.GetUserAsync();
	    if (users?.Value == null) { Console.WriteLine("No users found."); break; }
	    foreach (var d in users.Value)
		Console.WriteLine($"  {d.DisplayName} ({d.Id})");
	    break;

	case 2:
	    Console.WriteLine("\nListing devices...");
	    var devices = await GraphHelper.GetDeviceAsync();
	    if (devices?.Value == null) { Console.WriteLine("No devices found."); break; }
	    foreach (var d in devices.Value)
		Console.WriteLine($"  {d.DisplayName} ({d.Id})");
	    break;

	case 3:
	    Console.WriteLine("\nListing device configurations...");
	    var configs = await GraphHelper.GetDeviceConfigsAsync();
	    if (configs?.Value == null) { Console.WriteLine("No device configurations found."); break; }
	    foreach (var c in configs.Value)
		Console.WriteLine($"  {c.DisplayName} ({c.Id})");
	    break;

	case 4:
	    Console.WriteLine("\nListing device configuration statuses...");
	    var allConfigs = await GraphHelper.GetDeviceConfigsAsync();
	    if (allConfigs?.Value == null) { Console.WriteLine("No configurations found."); break; }
	    foreach (var c in allConfigs.Value)
	    {
		Console.WriteLine($"\n  Configuration: {c.DisplayName}");
		var statuses = await GraphHelper.GetDeviceConfigStatusesAsync(c.Id!);
		if (statuses?.Value == null) { Console.WriteLine(" No status data."); continue; }
		foreach (var s in statuses.Value)
		    Console.WriteLine($"    Device {s.DeviceDisplayName}: {s.Status}");
	    }
	    break;

	case 5:
	    Console.WriteLine("Settings Catalog configurations assigned to non-admin devices:");
	    var json = await GraphHelper.GetSettingsCatalogPoliciesRawAsync();
	    Console.WriteLine(json);
	    break;

	default:
	    Console.WriteLine("Invalid choice! Please try again.");
	    break;
    }
}
