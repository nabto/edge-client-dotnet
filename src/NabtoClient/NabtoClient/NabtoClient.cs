using Nabto.Edge.Client;
using System;
using Microsoft.Extensions.Logging;

namespace Nabto.Edge.Client;



public interface NabtoClient {
    public static NabtoClient Create() {
        return Impl.NabtoClient.Create();
    }
    public string GetVersion();

    public string CreatePrivateKey();

    public Connection CreateConnection();

    public void SetLogger(ILogger logger);
}
