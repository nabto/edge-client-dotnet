# Nabto Edge Client SDK .NET Library

This code is still in development, expect breaking changes to occur in future
versions.

## Compatible C# environments

Linux x86_64
Windows x86_64
Macos intel, arm

Other environments can be supported if the correct .so, .dll or .dylib is
available when running the resulting app/code.

## Example usage of the library

The following is a minimal example of how the code can be used, it can probably
be used in many other ways in your C# projects.
```
dotnet new console -n MyDemo
cd MyDemo
dotnet add reference path/to/src/NabtoClient
```

samples/Minimal/Program.cs:
```
using Nabto.Edge.Client;

var client = Nabto.Edge.Client.NabtoClient.Create();
var connection = client.CreateConnection();
connection.SetOptions(new ConnectionOptions {ProductId = "...", DeviceId = "...", ServerConnectToken = "...", PrivateKey = "..."});
await connection.ConnectAsync();
var coapRequest = connection.CreateCoapRequest("GET", "/path");
var response = await coapRequest.ExecuteAsync();
Console.WriteLine(response.GetResponsePayload());
```
