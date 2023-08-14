using Nabto.Edge.Client;
using Microsoft.Extensions.Logging;

var client = Nabto.Edge.Client.INabtoClient.Create();
using var loggerFactory = LoggerFactory.Create(builder => builder.AddConsole());
var logger = loggerFactory.CreateLogger<INabtoClient>();
client.SetLogger(logger);
var connection = client.CreateConnection();
connection.SetOptions(new ConnectionOptions { ProductId = "...", DeviceId = "...", ServerConnectToken = "...", PrivateKey = "..." });
await connection.ConnectAsync();
var coapRequest = connection.CreateCoapRequest("GET", "/path");
var response = await coapRequest.ExecuteAsync();
Console.WriteLine(System.Text.Encoding.UTF8.GetString(response.GetResponsePayload()));
