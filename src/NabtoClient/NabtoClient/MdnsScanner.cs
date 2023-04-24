namespace Nabto.Edge.Client;

/**
 * <summary>
 * The result of an mDNS discovery request.
 * </summary>
 */
public class MdnsResult {

    /**
     * <summary>
     * Action which is associated with a result. This is used together
     * with the service instance name to manipulate the list of device.
     *
     * <list type="bullet">
     * <item>
     *   <description>
     *     `ADD`: Adding a new result
     *   </description>
     * </item>
     * <item>
     *   <description>
     *     `UPDATE`: Updating existing result
     *   </description>
     * </item>
     * <item>
     *   <description>
     *     `REMOVE`: Removing existing result
     *   </description>
     * </item>
     * </list>
     * </summary>
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
 * <summary>
 * An MdnsScanner scans for local mDNS enabled devices.
 * </summary>
 */
public interface MdnsScanner {

    /**
     * <summary>
     * The delegate is invoked when an mDNS result is ready.
     *
     * </summary>
     * <param name="mdnsResult"> The callback result.</param>
     */
    public delegate void ResultHandler(MdnsResult mdnsResult);


    /**
     * <summary>
     * Start the scan for local devices using mDNS.
     * </summary>
     */
    public void Start();

    /* TODO:
     *
     */
    public ResultHandler? Handlers { get; set; }
}
