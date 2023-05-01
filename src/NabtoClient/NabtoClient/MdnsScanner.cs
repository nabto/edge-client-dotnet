namespace Nabto.Edge.Client;

/**
 * <summary>
 * The result of an mDNS discovery request.
 * </summary>
 */
public class MdnsResult {

    /**
     * <summary>
     * Actions emitted by device to manipulate the mDNS service cache in the client. Applies to the service identified by serviceInstanceName in the result.
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

    /**
     * <summary>
     * The service instance name. Can be considered a globally unique primary key for the announced service and used for maintaining a service cache in the client, identifying each entry. The provided action in the result specifies how the cache should be updated for this service.
     * </summary>
     */
    public string? ServiceInstanceName { get; set; }

    /**
     * <summary>
     * Product id, nil if not set in received result.
     * </summary>
     */
    public string? ProductId { get; set; }

    /**
     * <summary>
     * Device id, nil if not set in received result.
     * </summary>
     */
    public string? DeviceId { get; set; }

    /**
     * <summary>
     * The action indicating how this result should be used for updating the client’s service cache.
     * </summary>
     */
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

    /**
     * <summary>Access result handlers.</summary>
     */
    public ResultHandler? Handlers { get; set; }
}
