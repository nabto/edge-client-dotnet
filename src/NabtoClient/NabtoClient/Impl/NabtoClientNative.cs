using System.Runtime.InteropServices;
using System.Text;

namespace Nabto.Edge.Client.Impl;

public unsafe class NabtoClientNative
{
    const string _dllName = "nabto_client";


    // not simply using a string as .net then deallocates the non heap allocated const char*.
    [DllImport(_dllName, CharSet = CharSet.Ansi, ExactSpelling = true, EntryPoint = "nabto_client_version")]
    static extern byte* nabto_client_version_native();

    public static string nabto_client_version()
    {
        return constCharPointerToString(nabto_client_version_native());
    }

    // Nabto Client
    [DllImport(_dllName, CharSet = CharSet.Ansi, ExactSpelling = true)]
    public static extern IntPtr nabto_client_new();

    [DllImport(_dllName, CharSet = CharSet.Ansi, ExactSpelling = true)]
    public static extern void nabto_client_free(IntPtr context);

    [DllImport(_dllName, CharSet = CharSet.Ansi, ExactSpelling = true)]
    public static extern void nabto_client_stop(IntPtr context);

    [DllImport(_dllName, CharSet = CharSet.Ansi, ExactSpelling = true, EntryPoint = "nabto_client_create_private_key")]
    public static extern int nabto_client_create_private_key_native(IntPtr context, out byte* privateKey);

    public static int nabto_client_create_private_key(IntPtr client, out string privateKey)
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
    public delegate void LogCallbackFunc(IntPtr logMessage, IntPtr userData);

    [DllImport(_dllName, CharSet = CharSet.Ansi, ExactSpelling = true)]
    public static extern int nabto_client_set_log_callback(IntPtr context, [MarshalAs(UnmanagedType.FunctionPtr)] LogCallbackFunc cb, IntPtr userData);

    [DllImport(_dllName, CharSet = CharSet.Ansi, ExactSpelling = true)]
    public static extern int nabto_client_set_log_level(IntPtr context, [MarshalAs(UnmanagedType.LPUTF8Str)] string logLevel);

    [DllImport(_dllName, CharSet = CharSet.Ansi, ExactSpelling = true)]
    public static extern int nabto_client_log_message_get_severity(IntPtr logMessage);

    [DllImport(_dllName, CharSet = CharSet.Ansi, ExactSpelling = true, EntryPoint = "nabto_client_log_message_get_message")]
    static extern byte* nabto_client_log_message_get_message_native(IntPtr logMessage);

    public static string nabto_client_log_message_get_message(IntPtr logMessage)
    {
        return constCharPointerToString(nabto_client_log_message_get_message_native(logMessage));
    }


    /***************
     * Connections *
     ***************/
    [DllImport(_dllName, CharSet = CharSet.Ansi, ExactSpelling = true)]
    public static extern IntPtr nabto_client_connection_new(IntPtr client);

    [DllImport(_dllName, CharSet = CharSet.Ansi, ExactSpelling = true)]
    public static extern void nabto_client_connection_free(IntPtr connection);

    [DllImport(_dllName, CharSet = CharSet.Ansi, ExactSpelling = true)]
    public static extern int nabto_client_connection_set_options(IntPtr connection, [MarshalAs(UnmanagedType.LPUTF8Str)] string options);

    [DllImport(_dllName, CharSet = CharSet.Ansi, ExactSpelling = true, EntryPoint = "nabto_client_connection_get_client_fingerprint")]
    public static extern int nabto_client_connection_get_client_fingerprint_native(IntPtr connection, out byte* fingerprint);

    public static int nabto_client_connection_get_client_fingerprint(IntPtr connection, out string fingerprint)
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
    public static extern int nabto_client_connection_get_device_fingerprint_native(IntPtr connection, out byte* fingerprint);

    public static int nabto_client_connection_get_device_fingerprint(IntPtr connection, out string fingerprint)
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
    public static extern void nabto_client_connection_connect(IntPtr connection, IntPtr future);

    [DllImport(_dllName, CharSet = CharSet.Ansi, ExactSpelling = true)]
    public static extern void nabto_client_connection_close(IntPtr connection, IntPtr future);

    /***********
     * futures *
     ***********/
    [DllImport(_dllName, CharSet = CharSet.Ansi, ExactSpelling = true)]
    public static extern IntPtr nabto_client_future_new(IntPtr client);

    [DllImport(_dllName, CharSet = CharSet.Ansi, ExactSpelling = true)]
    public static extern void nabto_client_future_free(IntPtr future);

    public delegate void FutureCallbackFunc(IntPtr future, int ec, IntPtr userData);

    [DllImport(_dllName, CharSet = CharSet.Ansi, ExactSpelling = true)]
    public static extern void nabto_client_future_set_callback(IntPtr future, [MarshalAs(UnmanagedType.FunctionPtr)] FutureCallbackFunc cb, IntPtr userData);


    /*****************
     * CoAP Requests *
     *****************/
    [DllImport(_dllName, CharSet = CharSet.Ansi, ExactSpelling = true)]
    public static extern IntPtr nabto_client_coap_new(IntPtr connection, [MarshalAs(UnmanagedType.LPUTF8Str)] string method, [MarshalAs(UnmanagedType.LPUTF8Str)] string path);

    [DllImport(_dllName, CharSet = CharSet.Ansi, ExactSpelling = true)]
    public static extern void nabto_client_coap_free(IntPtr coap);

    [DllImport(_dllName, CharSet = CharSet.Ansi, ExactSpelling = true)]
    public static extern void nabto_client_coap_stop(IntPtr coap);

    // UIntPtr has the same number og bits as size_t
    [DllImport(_dllName, CharSet = CharSet.Ansi, ExactSpelling = true, EntryPoint = "nabto_client_coap_set_request_payload")]
    public static extern int nabto_client_coap_set_request_payload_native(IntPtr coap, ushort contentFormat, byte* payload, UIntPtr payloadLength);

    public static int nabto_client_coap_set_request_payload(IntPtr coap, ushort contentFormat, byte[] payload)
    {
        int ec;
        UIntPtr length = (UIntPtr)payload.Length;
        fixed(byte* start = &payload[0]) {
            ec = nabto_client_coap_set_request_payload_native(coap, contentFormat, start, length);
        }
        return ec;
    }

    [DllImport(_dllName, CharSet = CharSet.Ansi, ExactSpelling = true)]
    public static extern void nabto_client_coap_execute(IntPtr coap, IntPtr future);

    [DllImport(_dllName, CharSet = CharSet.Ansi, ExactSpelling = true)]
    public static extern int nabto_client_coap_get_response_status_code(IntPtr coap, out ushort statusCode);

    [DllImport(_dllName, CharSet = CharSet.Ansi, ExactSpelling = true)]
    public static extern int nabto_client_coap_get_response_content_format(IntPtr coap, out ushort contentType);

    [DllImport(_dllName, CharSet = CharSet.Ansi, ExactSpelling = true, EntryPoint = "nabto_client_coap_get_response_payload")]
    public static extern int nabto_client_coap_get_response_payload_native(IntPtr coap, out byte* payload, out UIntPtr payloadLength);

    public static int nabto_client_coap_get_response_payload(IntPtr coap, out byte[] payload)
    {
        byte* p;
        UIntPtr pLength;
        int ec = nabto_client_coap_get_response_payload_native(coap, out p, out pLength);
        if (ec == 0)
        {
            payload = new byte[pLength];
            for (int i = 0; i < ((uint)pLength); i++) {
                payload[i] = p[i];
            }
        } else {
            payload = new byte[0];
        }
        return ec;
    }



    /*************
     * Streaming *
     *************/

    [DllImport(_dllName, CharSet = CharSet.Ansi, ExactSpelling = true)]
    public static extern IntPtr nabto_client_stream_new(IntPtr connection);

    [DllImport(_dllName, CharSet = CharSet.Ansi, ExactSpelling = true)]
    public static extern void nabto_client_stream_free(IntPtr stream);

[DllImport(_dllName, CharSet = CharSet.Ansi, ExactSpelling = true)]
public static extern void nabto_client_stream_stop(IntPtr stream);

[DllImport(_dllName, CharSet = CharSet.Ansi, ExactSpelling = true)]
public static extern void nabto_client_stream_open(IntPtr stream, IntPtr future, UInt32 port);

[DllImport(_dllName, CharSet = CharSet.Ansi, ExactSpelling = true)]
public static extern void nabto_client_stream_read_all(IntPtr stream, IntPtr future, IntPtr buffer, UIntPtr bufferLength, out UIntPtr readLength);

[DllImport(_dllName, CharSet = CharSet.Ansi, ExactSpelling = true)]
public static extern void nabto_client_stream_read_some(IntPtr stream, IntPtr future, IntPtr buffer, UIntPtr bufferLength, out UIntPtr readLength);

[DllImport(_dllName, CharSet = CharSet.Ansi, ExactSpelling = true)]
public static extern void nabto_client_stream_write(IntPtr stream, IntPtr future, IntPtr buffer, UIntPtr bufferLength);

[DllImport(_dllName, CharSet = CharSet.Ansi, ExactSpelling = true)]
public static extern void nabto_client_stream_close(IntPtr stream, IntPtr future);



    // util functions
    [DllImport(_dllName, CharSet = CharSet.Ansi, ExactSpelling = true)]
    public static extern void nabto_client_string_free(byte* s);


    /******************
     * Error Handling *
     ******************/
    [DllImport(_dllName, CharSet = CharSet.Ansi, ExactSpelling = true, EntryPoint = "nabto_client_error_get_message")]
    static extern byte* nabto_client_error_get_message_native(int ec);

    [DllImport(_dllName, CharSet = CharSet.Ansi, ExactSpelling = true, EntryPoint = "nabto_client_error_get_string")]
    static extern byte* nabto_client_error_get_string_native(int ec);


    [DllImport(_dllName)] public static extern int NABTO_CLIENT_EC_OK_value();
    [DllImport(_dllName)] public static extern int NABTO_CLIENT_EC_ABORTED_value();
    [DllImport(_dllName)] public static extern int NABTO_CLIENT_EC_BAD_RESPONSE_value();
    [DllImport(_dllName)] public static extern int NABTO_CLIENT_EC_BAD_REQUEST_value();
    [DllImport(_dllName)] public static extern int NABTO_CLIENT_EC_CLOSED_value();
    [DllImport(_dllName)] public static extern int NABTO_CLIENT_EC_DNS_value();
    [DllImport(_dllName)] public static extern int NABTO_CLIENT_EC_EOF_value();
    [DllImport(_dllName)] public static extern int NABTO_CLIENT_EC_FORBIDDEN_value();
    [DllImport(_dllName)] public static extern int NABTO_CLIENT_EC_FUTURE_NOT_RESOLVED_value();
    [DllImport(_dllName)] public static extern int NABTO_CLIENT_EC_INVALID_ARGUMENT_value();
    [DllImport(_dllName)] public static extern int NABTO_CLIENT_EC_INVALID_STATE_value();
    [DllImport(_dllName)] public static extern int NABTO_CLIENT_EC_NOT_CONNECTED_value();
    [DllImport(_dllName)] public static extern int NABTO_CLIENT_EC_NOT_FOUND_value();
    [DllImport(_dllName)] public static extern int NABTO_CLIENT_EC_NOT_IMPLEMENTED_value();
    [DllImport(_dllName)] public static extern int NABTO_CLIENT_EC_NO_CHANNELS_value();
    [DllImport(_dllName)] public static extern int NABTO_CLIENT_EC_NO_DATA_value();
    [DllImport(_dllName)] public static extern int NABTO_CLIENT_EC_OPERATION_IN_PROGRESS_value();
    [DllImport(_dllName)] public static extern int NABTO_CLIENT_EC_PARSE_value();
    [DllImport(_dllName)] public static extern int NABTO_CLIENT_EC_PORT_IN_USE_value();
    [DllImport(_dllName)] public static extern int NABTO_CLIENT_EC_STOPPED_value();
    [DllImport(_dllName)] public static extern int NABTO_CLIENT_EC_TIMEOUT_value();
    [DllImport(_dllName)] public static extern int NABTO_CLIENT_EC_UNKNOWN_value();
    [DllImport(_dllName)] public static extern int NABTO_CLIENT_EC_NONE_value();
    [DllImport(_dllName)] public static extern int NABTO_CLIENT_EC_NOT_ATTACHED_value();
    [DllImport(_dllName)] public static extern int NABTO_CLIENT_EC_TOKEN_REJECTED_value();
    [DllImport(_dllName)] public static extern int NABTO_CLIENT_EC_COULD_BLOCK_value();
    [DllImport(_dllName)] public static extern int NABTO_CLIENT_EC_UNAUTHORIZED_value();
    [DllImport(_dllName)] public static extern int NABTO_CLIENT_EC_TOO_MANY_REQUESTS_value();
    [DllImport(_dllName)] public static extern int NABTO_CLIENT_EC_UNKNOWN_PRODUCT_ID_value();
    [DllImport(_dllName)] public static extern int NABTO_CLIENT_EC_UNKNOWN_DEVICE_ID_value();
    [DllImport(_dllName)] public static extern int NABTO_CLIENT_EC_UNKNOWN_SERVER_KEY_value();
    [DllImport(_dllName)] public static extern int NABTO_CLIENT_EC_CONNECTION_REFUSED_value();
    [DllImport(_dllName)] public static extern int NABTO_CLIENT_EC_PRIVILEGED_PORT_value();

    public static string nabto_client_error_get_message(int ec)
    {
        return constCharPointerToString(nabto_client_error_get_message_native(ec));
    }

    public static string nabto_client_error_get_string(int ec)
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
