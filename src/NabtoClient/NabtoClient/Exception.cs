namespace Nabto.Edge.Client;
public class NotConnectedException : Exception {
    public NotConnectedException() :
        base("The connection is not connected")
    {
    }
}
