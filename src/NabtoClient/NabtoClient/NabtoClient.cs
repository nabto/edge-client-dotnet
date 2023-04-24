using Nabto.Edge.Client;
using System;
using Microsoft.Extensions.Logging;

namespace Nabto.Edge.Client;


/**
 * <summary>
 * This interface is the main entry point for the Nabto Edge Client SDK .Net wrapper.
 *
 * It enables you to create a connection object, used to connect to a Nabto Edge Embedded device. And it provides misc
 * support functions: Create a private key (mandatory to later connect to a device), control logging, get SDK version.
 * The Client object must be kept alive for the duration of all connections created from it.
 * </summary>
 */
public interface NabtoClient {

    /**
     * <summary>
     * Create a new instance of the Nabto Edge client.
     * </summary>
     */
    public static NabtoClient Create() {
        return Impl.NabtoClientImpl.Create();
    }

    /**
     * <summary>
     * Get the underlying SDK version.
     *
     * </summary>
     * <returns>
     *     the SDK version, e.g. 5.2.0
     * </returns>
     */
    public string GetVersion();

    /**
     * <summary>
     * Create a private key and return the private key as a pem encoded string.
     *
     * The result is normally stored in a device specific secure location and retrieved whenever a new connection
     * is established, passed on to a Connection object using `setPrivateKey()`.
     *
     * </summary>
     * <exception cref="NabtoException">FAILED if key could not be created</exception>
     * <returns>
     *     the private key as a pem encoded string.
     * </returns>
     */
    public string CreatePrivateKey();

    /**
     * <summary>
     * Create a connection object.
     *
     * The created connection can then be configured and opened. Returned object must be kept alive while in use.
     *
     * </summary>
     * <exception cref="NabtoException">ALLOCATION_ERROR if the underlying SDK fails creating a connection object</exception>
     */
    public Connection CreateConnection();

    /**
     * <summary>
     * Create an mDNS scanner to discover local devices. Returned object must be kept alive while in use.
     *
     * </summary>
     * <param name="subType"> the mDNS subtype to scan for: If the empty string, the mDNS subtype
     * `_nabto._udp.local` is located; if subtype is specified, `[subtype]._sub._nabto._udp.local` is located.</param>
     * <exception cref="NabtoException">NabtoEdgeClientError</exception>
     * <returns>
     *     The MdnsScanner
     * </returns>
     */
    public MdnsScanner CreateMdnsScanner(string subtype = "");

    /**
     * <summary>
     * Set a logger for getting log information from the SDK.
     *
     * </summary>
     * <param name="logger"> The logger to set.</param>
     */
    public void SetLogger(ILogger logger);
}
