namespace Nabto.Edge.ClientIam;

/**
  * <summary>Pairing mode - see https://docs.nabto.com/developer/guides/iam/pairing.html#modes. Used when querying a device for details, specifically which pairing modes are enabled on the active connection.</summary>
  */
public enum PairingMode
{
    /**
     * <summary>Allows any client to pair that knows the common open pairing password.</summary>
     */
    PASSWORD_OPEN,

    /**
     * <summary>Allows any client to pair that is on the same local network as the device.</summary>
     */
    LOCAL_OPEN,

    /**
     * <summary>Allows an invited user to pair, ie a user that knows a username and the specific password for this user.</summary>
     */
    PASSWORD_INVITE,

    /**
     * <summary>The as Local Open pairing mode - except that after the first user is paired, this pairing mode is disabled.</summary>
     */
    LOCAL_INITIAL
};

/**
 * <summary>This class describes a device, ie a C# object representation of the CBOR object return by the /iam/pairing CoAP endpoint 
 * https://docs.nabto.com/developer/api-reference/coap/iam/pairing.html</summary>
 */
public class DeviceDetails
{
    /**
     * <summary>Pairing modes supported by the device.</summary>
     */
    public HashSet<PairingMode> Modes = new HashSet<PairingMode>();

    /**
     * <summary>Nabto Edge SDK version.</summary>
     */
    public string? NabtoVersion;
    /**
     * <summary>Application version as defined by the device.</summary>
     */
    public string? AppVersion;
    /**
     * <summary>Application name as defined by the device.</summary>
     */
    public string? AppName;
    /**
     * <summary>Nabto Edge device Product ID.</summary>
     */
    public string? ProductId;
    /**
     * <summary>Nabto Edge device Device ID.</summary>
     */
    public string? DeviceId;
    /**
     * <summary>Friendly (readable) name assigned to this name.</summary>
     */
    public string? FriendlyName;
};
