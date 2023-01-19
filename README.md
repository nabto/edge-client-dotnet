# Nabto Edge Client SDK .NET Library

This code is still in development, expect breaking changes to occur in future versions.

# Example usage of the library

dotnet new console -n MyDemo
cd MyDemo
dotnet add reference path/to/src/NabtoClient

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
