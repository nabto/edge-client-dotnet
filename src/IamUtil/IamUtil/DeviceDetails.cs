namespace Nabto.Edge.Client;

public enum PairingMode
{
    PASSWORD_OPEN,
    LOCAL_OPEN,
    PASSWORD_INVITE,
    LOCAL_INITIAL
};

public class DeviceDetails
{
    public HashSet<PairingMode> Modes = new HashSet<PairingMode>();

    public string? NabtoVersion;
    public string? AppVersion;
    public string? AppName;
    public string? ProductId;
    public string? DeviceId;
    public string? FriendlyName;
};