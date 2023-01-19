using Nabto.Edge.Client;

var client = Nabto.Edge.Client.NabtoClient.Create();
var connection = client.CreateConnection();
connection.SetOptions(new ConnectionOptions {ProductId = "...", DeviceId = "...", ServerConnectToken = "...", PrivateKey = "..."});
await connection.ConnectAsync();
var coapRequest = connection.CreateCoapRequest("GET", "/path");
var response = await coapRequest.ExecuteAsync();
Console.WriteLine(response.GetResponsePayload());
