namespace Nabto.Edge.Client;

/**
 * The result of an mDNS discovery request.
 */
public class MdnsResult {

    /**
     * Action which is associated with a result. This is used together
     * with the service instance name to manipulate the list of device.
     *
     * - `ADD`: Adding a new result
     * - `UPDATE`: Updating existing result
     * - `REMOVE`: Removing existing result
     */
    public enum MdnsAction {
        ADD = 0,
        UPDATE = 1,
        REMOVE = 2
    }

    /* TODO
     */
    public string? ServiceInstanceName { get; set; }
    public string? ProductId { get; set; }
    public string? DeviceId { get; set; }
    public MdnsAction Action { get; set; }
};

/**
 * An MdnsScanner scans for local mDNS enabled devices.
 */
public interface MdnsScanner {

    /**
     * The delegate is invoked when an mDNS result is ready.
     *
     * @param mdnsResult The callback result.
     */
    public delegate void ResultHandler(MdnsResult mdnsResult);


    /**
     * Start the scan for local devices using mDNS.
     */
    public void Start();

    /* TODO:
     *
     */
    public ResultHandler? Handlers { get; set; }
}
