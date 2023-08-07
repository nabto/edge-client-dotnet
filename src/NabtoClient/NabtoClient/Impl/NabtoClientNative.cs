using System.Runtime.InteropServices;
using System.Text;

namespace Nabto.Edge.Client.Impl;

internal unsafe class NabtoClientNative
{
    const string _dllName = "nabto_client";

    internal enum NabtoClientConnectionType : int
    {
        Relay = 0,
        Direct
    }

    // not simply using a string as .net then deallocates the non heap allocated const char*.
    [DllImport(_dllName, CharSet = CharSet.Ansi, ExactSpelling = true, EntryPoint = "nabto_client_version")]
    static extern byte* nabto_client_version_native();

    internal static string nabto_client_version()
    {
        return constCharPointerToString(nabto_client_version_native());
    }

    // Nabto Client
    [DllImport(_dllName, CharSet = CharSet.Ansi, ExactSpelling = true)]
    internal static extern IntPtr nabto_client_new();

    [DllImport(_dllName, CharSet = CharSet.Ansi, ExactSpelling = true)]
    internal static extern void nabto_client_free(IntPtr context);

    [DllImport(_dllName, CharSet = CharSet.Ansi, ExactSpelling = true)]
    internal static extern void nabto_client_stop(IntPtr context);

    [DllImport(_dllName, CharSet = CharSet.Ansi, ExactSpelling = true, EntryPoint = "nabto_client_create_private_key")]
    internal static extern int nabto_client_create_private_key_native(IntPtr context, out byte* privateKey);

    internal static int nabto_client_create_private_key(IntPtr client, out string privateKey)
    {
        byte* data;
        int ec;
        privateKey = "";
        ec = nabto_client_create_private_key_native(client, out data);
        if (ec == 0)
        {
            privateKey = DecodeStringAndFree(data);
        }
        return ec;
    }


    /***********
     * Logging *
     ***********/
    internal delegate void LogCallbackFunc(IntPtr logMessage, IntPtr userData);

    [DllImport(_dllName, CharSet = CharSet.Ansi, ExactSpelling = true)]
    internal static extern int nabto_client_set_log_callback(IntPtr context, [MarshalAs(UnmanagedType.FunctionPtr)] LogCallbackFunc cb, IntPtr userData);

    [DllImport(_dllName, CharSet = CharSet.Ansi, ExactSpelling = true)]
    internal static extern int nabto_client_set_log_level(IntPtr context, [MarshalAs(UnmanagedType.LPUTF8Str)] string logLevel);

    [DllImport(_dllName, CharSet = CharSet.Ansi, ExactSpelling = true)]
    internal static extern int nabto_client_log_message_get_severity(IntPtr logMessage);

    [DllImport(_dllName, CharSet = CharSet.Ansi, ExactSpelling = true, EntryPoint = "nabto_client_log_message_get_message")]
    static extern byte* nabto_client_log_message_get_message_native(IntPtr logMessage);

    internal static string nabto_client_log_message_get_message(IntPtr logMessage)
    {
        return constCharPointerToString(nabto_client_log_message_get_message_native(logMessage));
    }


    /***************
     * Connections *
     ***************/
    [DllImport(_dllName, CharSet = CharSet.Ansi, ExactSpelling = true)]
    internal static extern IntPtr nabto_client_connection_new(IntPtr client);

    [DllImport(_dllName, CharSet = CharSet.Ansi, ExactSpelling = true)]
    internal static extern void nabto_client_connection_free(IntPtr connection);

    [DllImport(_dllName, CharSet = CharSet.Ansi, ExactSpelling = true)]
    internal static extern void nabto_client_connection_stop(IntPtr connection);

    [DllImport(_dllName, CharSet = CharSet.Ansi, ExactSpelling = true)]
    internal static extern int nabto_client_connection_set_options(IntPtr connection, [MarshalAs(UnmanagedType.LPUTF8Str)] string options);

    [DllImport(_dllName, CharSet = CharSet.Ansi, ExactSpelling = true)]
    internal static extern int nabto_client_connection_get_options(IntPtr connection, [MarshalAs(UnmanagedType.LPUTF8Str)] out string options);


    [DllImport(_dllName, CharSet = CharSet.Ansi, ExactSpelling = true, EntryPoint = "nabto_client_connection_get_client_fingerprint")]
    internal static extern int nabto_client_connection_get_client_fingerprint_native(IntPtr connection, out byte* fingerprint);

    internal static int nabto_client_connection_get_client_fingerprint(IntPtr connection, out string fingerprint)
    {
        byte* data;
        int ec;
        fingerprint = "";
        ec = nabto_client_connection_get_client_fingerprint_native(connection, out data);
        if (ec == 0)
        {
            fingerprint = DecodeStringAndFree(data);
        }
        return ec;
    }

    [DllImport(_dllName, CharSet = CharSet.Ansi, ExactSpelling = true, EntryPoint = "nabto_client_connection_get_device_fingerprint")]
    internal static extern int nabto_client_connection_get_device_fingerprint_native(IntPtr connection, out byte* fingerprint);

    internal static int nabto_client_connection_get_device_fingerprint(IntPtr connection, out string fingerprint)
    {
        byte* data;
        int ec;
        fingerprint = "";
        ec = nabto_client_connection_get_device_fingerprint_native(connection, out data);
        if (ec == 0)
        {
            fingerprint = DecodeStringAndFree(data);
        }
        return ec;
    }

    [DllImport(_dllName, CharSet = CharSet.Ansi, ExactSpelling = true)]
    internal static extern void nabto_client_connection_connect(IntPtr connection, IntPtr future);

    [DllImport(_dllName, CharSet = CharSet.Ansi, ExactSpelling = true)]
    internal static extern void nabto_client_connection_close(IntPtr connection, IntPtr future);

    [DllImport(_dllName, CharSet = CharSet.Ansi, ExactSpelling = true)]
    internal static extern void nabto_client_connection_password_authenticate(IntPtr connection, [MarshalAs(UnmanagedType.LPUTF8Str)] string username, [MarshalAs(UnmanagedType.LPUTF8Str)] string password, IntPtr future);


    [DllImport(_dllName, CharSet = CharSet.Ansi, ExactSpelling = true)]
    internal static extern int nabto_client_connection_get_local_channel_error_code(IntPtr connection);

    [DllImport(_dllName, CharSet = CharSet.Ansi, ExactSpelling = true)]
    internal static extern int nabto_client_connection_get_remote_channel_error_code(IntPtr connection);

    [DllImport(_dllName, CharSet = CharSet.Ansi, ExactSpelling = true)]
    internal static extern int nabto_client_connection_get_direct_candidates_channel_error_code(IntPtr connection);

    [DllImport(_dllName, CharSet = CharSet.Ansi, ExactSpelling = true)]
    internal static extern int nabto_client_connection_get_type(IntPtr connection, out int type);


    /*********************
     * Connection Events *
     *********************/

    [DllImport(_dllName, CharSet = CharSet.Ansi, ExactSpelling = true)]
    internal static extern void nabto_client_listener_connection_event(IntPtr listener, IntPtr future, out int e);

    [DllImport(_dllName, CharSet = CharSet.Ansi, ExactSpelling = true)]
    internal static extern int nabto_client_connection_events_init_listener(IntPtr connection, IntPtr listener);

    [DllImport(_dllName, CharSet = CharSet.Ansi, ExactSpelling = true)]
    internal static extern int NABTO_CLIENT_CONNECTION_EVENT_CONNECTED_value();

    [DllImport(_dllName, CharSet = CharSet.Ansi, ExactSpelling = true)]
    internal static extern int NABTO_CLIENT_CONNECTION_EVENT_CLOSED_value();

    [DllImport(_dllName, CharSet = CharSet.Ansi, ExactSpelling = true)]
    internal static extern int NABTO_CLIENT_CONNECTION_EVENT_CHANNEL_CHANGED_value();



    /***********
     * futures *
     ***********/
    [DllImport(_dllName, CharSet = CharSet.Ansi, ExactSpelling = true)]
    internal static extern IntPtr nabto_client_future_new(IntPtr client);

    [DllImport(_dllName, CharSet = CharSet.Ansi, ExactSpelling = true)]
    internal static extern void nabto_client_future_free(IntPtr future);

    internal delegate void FutureCallbackFunc(IntPtr future, int ec, IntPtr userData);

    [DllImport(_dllName, CharSet = CharSet.Ansi, ExactSpelling = true)]
    internal static extern void nabto_client_future_set_callback(IntPtr future, [MarshalAs(UnmanagedType.FunctionPtr)] FutureCallbackFunc cb, IntPtr userData);


    /*************
     * Listeners *
     *************/
    [DllImport(_dllName, CharSet = CharSet.Ansi, ExactSpelling = true)]
    internal static extern IntPtr nabto_client_listener_new(IntPtr context);

    [DllImport(_dllName, CharSet = CharSet.Ansi, ExactSpelling = true)]
    internal static extern void nabto_client_listener_free(IntPtr listener);

    [DllImport(_dllName, CharSet = CharSet.Ansi, ExactSpelling = true)]
    internal static extern void nabto_client_listener_stop(IntPtr listener);


    /*****************
     * CoAP Requests *
     *****************/
    [DllImport(_dllName, CharSet = CharSet.Ansi, ExactSpelling = true)]
    internal static extern IntPtr nabto_client_coap_new(IntPtr connection, [MarshalAs(UnmanagedType.LPUTF8Str)] string method, [MarshalAs(UnmanagedType.LPUTF8Str)] string path);

    [DllImport(_dllName, CharSet = CharSet.Ansi, ExactSpelling = true)]
    internal static extern void nabto_client_coap_free(IntPtr coap);

    [DllImport(_dllName, CharSet = CharSet.Ansi, ExactSpelling = true)]
    internal static extern void nabto_client_coap_stop(IntPtr coap);

    // UIntPtr has the same number og bits as size_t
    [DllImport(_dllName, CharSet = CharSet.Ansi, ExactSpelling = true, EntryPoint = "nabto_client_coap_set_request_payload")]
    internal static extern int nabto_client_coap_set_request_payload_native(IntPtr coap, ushort contentFormat, byte* payload, UIntPtr payloadLength);

    internal static int nabto_client_coap_set_request_payload(IntPtr coap, ushort contentFormat, byte[] payload)
    {
        int ec;
        UIntPtr length = (UIntPtr)payload.Length;
        fixed (byte* start = &payload[0])
        {
            ec = nabto_client_coap_set_request_payload_native(coap, contentFormat, start, length);
        }
        return ec;
    }

    [DllImport(_dllName, CharSet = CharSet.Ansi, ExactSpelling = true)]
    internal static extern void nabto_client_coap_execute(IntPtr coap, IntPtr future);

    [DllImport(_dllName, CharSet = CharSet.Ansi, ExactSpelling = true)]
    internal static extern int nabto_client_coap_get_response_status_code(IntPtr coap, out ushort statusCode);

    [DllImport(_dllName, CharSet = CharSet.Ansi, ExactSpelling = true)]
    internal static extern int nabto_client_coap_get_response_content_format(IntPtr coap, out ushort contentType);

    [DllImport(_dllName, CharSet = CharSet.Ansi, ExactSpelling = true, EntryPoint = "nabto_client_coap_get_response_payload")]
    internal static extern int nabto_client_coap_get_response_payload_native(IntPtr coap, out byte* payload, out UIntPtr payloadLength);

    internal static int nabto_client_coap_get_response_payload(IntPtr coap, out byte[] payload)
    {
        byte* p;
        UIntPtr pLength;
        int ec = nabto_client_coap_get_response_payload_native(coap, out p, out pLength);
        if (ec == 0)
        {
            payload = new byte[pLength];
            for (int i = 0; i < ((uint)pLength); i++)
            {
                payload[i] = p[i];
            }
        }
        else
        {
            payload = new byte[0];
        }
        return ec;
    }



    /*************
     * Streaming *
     *************/

    [DllImport(_dllName, CharSet = CharSet.Ansi, ExactSpelling = true)]
    internal static extern IntPtr nabto_client_stream_new(IntPtr connection);

    [DllImport(_dllName, CharSet = CharSet.Ansi, ExactSpelling = true)]
    internal static extern void nabto_client_stream_free(IntPtr stream);

    [DllImport(_dllName, CharSet = CharSet.Ansi, ExactSpelling = true)]
    internal static extern void nabto_client_stream_stop(IntPtr stream);

    [DllImport(_dllName, CharSet = CharSet.Ansi, ExactSpelling = true)]
    internal static extern void nabto_client_stream_open(IntPtr stream, IntPtr future, UInt32 port);

    [DllImport(_dllName, CharSet = CharSet.Ansi, ExactSpelling = true)]
    internal static extern void nabto_client_stream_read_all(IntPtr stream, IntPtr future, IntPtr buffer, UIntPtr bufferLength, out UIntPtr readLength);

    [DllImport(_dllName, CharSet = CharSet.Ansi, ExactSpelling = true)]
    internal static extern void nabto_client_stream_read_some(IntPtr stream, IntPtr future, IntPtr buffer, UIntPtr bufferLength, out UIntPtr readLength);

    [DllImport(_dllName, CharSet = CharSet.Ansi, ExactSpelling = true)]
    internal static extern void nabto_client_stream_write(IntPtr stream, IntPtr future, IntPtr buffer, UIntPtr bufferLength);

    [DllImport(_dllName, CharSet = CharSet.Ansi, ExactSpelling = true)]
    internal static extern void nabto_client_stream_close(IntPtr stream, IntPtr future);

    /**************
     * Tunnelling *
     **************/
    [DllImport(_dllName, CharSet = CharSet.Ansi, ExactSpelling = true)]
    internal static extern IntPtr nabto_client_tcp_tunnel_new(IntPtr connection);

    [DllImport(_dllName, CharSet = CharSet.Ansi, ExactSpelling = true)]
    internal static extern void nabto_client_tcp_tunnel_free(IntPtr tunnel);

    [DllImport(_dllName, CharSet = CharSet.Ansi, ExactSpelling = true)]
    internal static extern void nabto_client_tcp_tunnel_stop(IntPtr tunnel);

    [DllImport(_dllName, CharSet = CharSet.Ansi, ExactSpelling = true)]
    internal static extern void nabto_client_tcp_tunnel_open(IntPtr tunnel, IntPtr future, [MarshalAs(UnmanagedType.LPUTF8Str)] string service, ushort localPort);

    [DllImport(_dllName, CharSet = CharSet.Ansi, ExactSpelling = true)]
    internal static extern void nabto_client_tcp_tunnel_close(IntPtr tunnel, IntPtr future);

    [DllImport(_dllName, CharSet = CharSet.Ansi, ExactSpelling = true)]
    internal static extern int nabto_client_tcp_tunnel_get_local_port(IntPtr tunnel, out ushort localPort);

    /********
     * MDNS *
     ********/
    [DllImport(_dllName, CharSet = CharSet.Ansi, ExactSpelling = true)]
    internal static extern int nabto_client_mdns_resolver_init_listener(IntPtr client, IntPtr listener, [MarshalAs(UnmanagedType.LPUTF8Str)] string subType);

    [DllImport(_dllName, CharSet = CharSet.Ansi, ExactSpelling = true)]
    internal static extern void nabto_client_listener_new_mdns_result(IntPtr listener, IntPtr future, out IntPtr mdnsResult);

    [DllImport(_dllName, CharSet = CharSet.Ansi, ExactSpelling = true)]
    internal static extern void nabto_client_mdns_result_free(IntPtr result);

    [DllImport(_dllName, CharSet = CharSet.Ansi, ExactSpelling = true, EntryPoint = "nabto_client_mdns_result_get_device_id")]
    internal static extern byte* nabto_client_mdns_result_get_device_id_native(IntPtr result);
    internal static string nabto_client_mdns_result_get_device_id(IntPtr result)
    {
        return constCharPointerToString(nabto_client_mdns_result_get_device_id_native(result));
    }

    [DllImport(_dllName, CharSet = CharSet.Ansi, ExactSpelling = true, EntryPoint = "nabto_client_mdns_result_get_product_id")]
    internal static extern byte* nabto_client_mdns_result_get_product_id_native(IntPtr result);
    internal static string nabto_client_mdns_result_get_product_id(IntPtr result)
    {
        return constCharPointerToString(nabto_client_mdns_result_get_product_id_native(result));
    }

    [DllImport(_dllName, CharSet = CharSet.Ansi, ExactSpelling = true, EntryPoint = "nabto_client_mdns_result_get_service_instance_name")]
    internal static extern byte* nabto_client_mdns_result_get_service_instance_name_native(IntPtr result);

    internal static string nabto_client_mdns_result_get_service_instance_name(IntPtr result)
    {
        return constCharPointerToString(nabto_client_mdns_result_get_service_instance_name_native(result));
    }


    [DllImport(_dllName, CharSet = CharSet.Ansi, ExactSpelling = true)]
    internal static extern byte* nabto_client_mdns_result_get_txt_items(IntPtr result);


    [DllImport(_dllName, CharSet = CharSet.Ansi, ExactSpelling = true)]
    internal static extern int nabto_client_mdns_result_get_action(IntPtr result);


    // util functions
    [DllImport(_dllName, CharSet = CharSet.Ansi, ExactSpelling = true)]
    internal static extern void nabto_client_string_free(byte* s);


    /******************
     * Error Handling *
     ******************/
    [DllImport(_dllName, CharSet = CharSet.Ansi, ExactSpelling = true, EntryPoint = "nabto_client_error_get_message")]
    static extern byte* nabto_client_error_get_message_native(int ec);

    [DllImport(_dllName, CharSet = CharSet.Ansi, ExactSpelling = true, EntryPoint = "nabto_client_error_get_string")]
    static extern byte* nabto_client_error_get_string_native(int ec);


    [DllImport(_dllName)] internal static extern int NABTO_CLIENT_EC_OK_value();
    [DllImport(_dllName)] internal static extern int NABTO_CLIENT_EC_BAD_RESPONSE_value();
    [DllImport(_dllName)] internal static extern int NABTO_CLIENT_EC_BAD_REQUEST_value();
    [DllImport(_dllName)] internal static extern int NABTO_CLIENT_EC_CLOSED_value();
    [DllImport(_dllName)] internal static extern int NABTO_CLIENT_EC_DNS_value();
    [DllImport(_dllName)] internal static extern int NABTO_CLIENT_EC_EOF_value();
    [DllImport(_dllName)] internal static extern int NABTO_CLIENT_EC_FORBIDDEN_value();
    [DllImport(_dllName)] internal static extern int NABTO_CLIENT_EC_FUTURE_NOT_RESOLVED_value();
    [DllImport(_dllName)] internal static extern int NABTO_CLIENT_EC_INVALID_ARGUMENT_value();
    [DllImport(_dllName)] internal static extern int NABTO_CLIENT_EC_INVALID_STATE_value();
    [DllImport(_dllName)] internal static extern int NABTO_CLIENT_EC_NOT_CONNECTED_value();
    [DllImport(_dllName)] internal static extern int NABTO_CLIENT_EC_NOT_FOUND_value();
    [DllImport(_dllName)] internal static extern int NABTO_CLIENT_EC_NOT_IMPLEMENTED_value();
    [DllImport(_dllName)] internal static extern int NABTO_CLIENT_EC_NO_CHANNELS_value();
    [DllImport(_dllName)] internal static extern int NABTO_CLIENT_EC_NO_DATA_value();
    [DllImport(_dllName)] internal static extern int NABTO_CLIENT_EC_OPERATION_IN_PROGRESS_value();
    [DllImport(_dllName)] internal static extern int NABTO_CLIENT_EC_PARSE_value();
    [DllImport(_dllName)] internal static extern int NABTO_CLIENT_EC_PORT_IN_USE_value();
    [DllImport(_dllName)] internal static extern int NABTO_CLIENT_EC_STOPPED_value();
    [DllImport(_dllName)] internal static extern int NABTO_CLIENT_EC_TIMEOUT_value();
    [DllImport(_dllName)] internal static extern int NABTO_CLIENT_EC_UNKNOWN_value();
    [DllImport(_dllName)] internal static extern int NABTO_CLIENT_EC_NONE_value();
    [DllImport(_dllName)] internal static extern int NABTO_CLIENT_EC_NOT_ATTACHED_value();
    [DllImport(_dllName)] internal static extern int NABTO_CLIENT_EC_TOKEN_REJECTED_value();
    [DllImport(_dllName)] internal static extern int NABTO_CLIENT_EC_COULD_BLOCK_value();
    [DllImport(_dllName)] internal static extern int NABTO_CLIENT_EC_UNAUTHORIZED_value();
    [DllImport(_dllName)] internal static extern int NABTO_CLIENT_EC_TOO_MANY_REQUESTS_value();
    [DllImport(_dllName)] internal static extern int NABTO_CLIENT_EC_UNKNOWN_PRODUCT_ID_value();
    [DllImport(_dllName)] internal static extern int NABTO_CLIENT_EC_UNKNOWN_DEVICE_ID_value();
    [DllImport(_dllName)] internal static extern int NABTO_CLIENT_EC_UNKNOWN_SERVER_KEY_value();
    [DllImport(_dllName)] internal static extern int NABTO_CLIENT_EC_CONNECTION_REFUSED_value();
    [DllImport(_dllName)] internal static extern int NABTO_CLIENT_EC_PRIVILEGED_PORT_value();
    [DllImport(_dllName)] internal static extern int NABTO_CLIENT_EC_DEVICE_INTERNAL_ERROR_value();

    internal static string nabto_client_error_get_message(int ec)
    {
        return constCharPointerToString(nabto_client_error_get_message_native(ec));
    }

    internal static string nabto_client_error_get_string(int ec)
    {
        return constCharPointerToString(nabto_client_error_get_string_native(ec));
    }


    /********************
     * Datatype Helpers *
     ********************/


    static String constCharPointerToString(byte* data)
    {
        byte* str = data;
        int length = 0;
        for (byte* i = str; *i != 0; i++, length++) ;
        var clrString = Encoding.UTF8.GetString(str, length);
        return clrString;
    }

    static String DecodeStringAndFree(byte* data)
    {
        byte* str = data;
        int length = 0;
        for (byte* i = str; *i != 0; i++, length++) ;
        var clrString = Encoding.UTF8.GetString(str, length);
        nabto_client_string_free(data);
        return clrString;
    }

};
