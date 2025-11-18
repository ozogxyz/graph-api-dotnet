Tested with .NET SDK 10.0.100.

```bash
dotnet user-secrets init
dotnet user-secrets set settings:clientSecret <client-secret>
dotnet restore
dotnet build
dotnet run
```
