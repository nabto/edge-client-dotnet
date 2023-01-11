namespace Nabto.Edge.Client;

public interface Connection
{

    public void SetOptions(string json);

    public string GetDeviceFingerprint();
    public string GetClientFingerprint();
    public Task ConnectAsync();

};
