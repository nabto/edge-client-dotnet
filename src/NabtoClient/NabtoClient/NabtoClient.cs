using Nabto.Edge.Client;
using System;

namespace Nabto.Edge.Client;



public interface NabtoClient {
    public static NabtoClient Create() {
        IntPtr ptr = Impl.NabtoClientNative.nabto_client_new();
        if (ptr == IntPtr.Zero) {
            throw new NullReferenceException();
        }
        return new Impl.NabtoClient(ptr);
    }
    public string GetVersion();

    public Connection CreateConnection();
}
