using Nabto.Edge.Client;
using System;
using Microsoft.Extensions.Logging;

namespace Nabto.Edge.Client;


/**
 * This interface is the main entry point for the Nabto Edge Client SDK .Net wrapper.
 *
 * It enables you to create a connection object, used to connect to a Nabto Edge Embedded device. And it provides misc
 * support functions: Create a private key (mandatory to later connect to a device), control logging, get SDK version.
 * The Client object must be kept alive for the duration of all connections created from it.
 */
public interface NabtoClient {

    /**
     * Create a new instance of the Nabto Edge client.
     */
    public static NabtoClient Create() {
        return Impl.NabtoClientImpl.Create();
    }

    /**
     * Get the underlying SDK version.
     *
     * @return the SDK version, e.g. 5.2.0
     */
    public string GetVersion();

    /**
     * Create a private key and return the private key as a pem encoded string.
     *
     * The result is normally stored in a device specific secure location and retrieved whenever a new connection
     * is established, passed on to a Connection object using `setPrivateKey()`.
     *
     * @throws NabtoEdgeClientError.FAILED if key could not be created
     * @return the private key as a pem encoded string.
     */
    public string CreatePrivateKey();

    /**
     * Create a connection object.
     *
     * The created connection can then be configured and opened. Returned object must be kept alive while in use.
     *
     * @throws NabtoEdgeClientError.ALLOCATION_ERROR if the underlying SDK fails creating a connection object
     */
    public Connection CreateConnection();

    /**
     * Create an mDNS scanner to discover local devices. Returned object must be kept alive while in use.
     *
     * @param subType the mDNS subtype to scan for: If the empty string, the mDNS subtype
     * `_nabto._udp.local` is located; if subtype is specified, `[subtype]._sub._nabto._udp.local` is located.
     * @throws NabtoEdgeClientError
     * @return The MdnsScanner
     */
    public MdnsScanner CreateMdnsScanner(string subtype = "");

    /**
     * Set a logger for getting log information from the SDK.
     *
     * @param logger The logger to set.
     */
    public void SetLogger(ILogger logger);
}
