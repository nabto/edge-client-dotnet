using System.Runtime.InteropServices;
using System.Text;

namespace Nabto.Edge.Client.Impl;

public unsafe class NabtoClientNative
{
    const string _dllName = "nabto_client";


    [DllImport(_dllName, CharSet = CharSet.Ansi, ExactSpelling = true)]
    // not simply using a string as .net then deallocates the non heap allocated const char*.
    static extern byte* nabto_client_version();

    // Nabto Client
    [DllImport(_dllName, CharSet = CharSet.Ansi, ExactSpelling = true)]
    public static extern IntPtr nabto_client_new();

    [DllImport(_dllName, CharSet = CharSet.Ansi, ExactSpelling = true)]
    public static extern void nabto_client_free(IntPtr context);

    [DllImport(_dllName, CharSet = CharSet.Ansi, ExactSpelling = true)]
    public static extern void nabto_client_stop(IntPtr context);

    [DllImport(_dllName, CharSet = CharSet.Ansi, ExactSpelling = true)]
    public static extern int nabto_client_create_private_key(IntPtr context, out byte* privateKey);

    public delegate void LogCallbackFunc(IntPtr logMessage, IntPtr userData);

    [DllImport(_dllName, CharSet = CharSet.Ansi, ExactSpelling = true)]
    public static extern int nabto_client_set_log_callback(IntPtr context, [MarshalAs(UnmanagedType.FunctionPtr)] LogCallbackFunc cb, IntPtr userData);

    [DllImport(_dllName, CharSet = CharSet.Ansi, ExactSpelling = true)]
    public static extern int nabto_client_set_log_level(IntPtr context, [MarshalAs(UnmanagedType.LPUTF8Str)] string logLevel);

    [DllImport(_dllName, CharSet = CharSet.Ansi, ExactSpelling = true)]
    public static extern int nabto_client_log_message_get_severity(IntPtr logMessage);

    [DllImport(_dllName, CharSet = CharSet.Ansi, ExactSpelling = true)]
    public static extern byte* nabto_client_log_message_get_message(IntPtr logMessage);



    // Connection
    [DllImport(_dllName, CharSet = CharSet.Ansi, ExactSpelling = true)]
    public static extern IntPtr nabto_client_connection_new(IntPtr client);

    [DllImport(_dllName, CharSet = CharSet.Ansi, ExactSpelling = true)]
    public static extern void nabto_client_connection_free(IntPtr connection);

    [DllImport(_dllName, CharSet = CharSet.Ansi, ExactSpelling = true)]
    public static extern int nabto_client_connection_set_options(IntPtr connection, [MarshalAs(UnmanagedType.LPUTF8Str)] string options);

    [DllImport(_dllName, CharSet = CharSet.Ansi, ExactSpelling = true)]
    public static extern int nabto_client_connection_get_client_fingerprint(IntPtr connection, out byte* fingerprint);

    [DllImport(_dllName, CharSet = CharSet.Ansi, ExactSpelling = true)]
    public static extern int nabto_client_connection_get_device_fingerprint(IntPtr connection, out byte* fingerprint);

    [DllImport(_dllName, CharSet = CharSet.Ansi, ExactSpelling = true)]
    public static extern void nabto_client_connection_connect(IntPtr connection, IntPtr future);


    // futures
    [DllImport(_dllName, CharSet = CharSet.Ansi, ExactSpelling = true)]
    public static extern IntPtr nabto_client_future_new(IntPtr client);

    [DllImport(_dllName, CharSet = CharSet.Ansi, ExactSpelling = true)]
    public static extern void nabto_client_future_free(IntPtr future);

    public delegate void FutureCallbackFunc(IntPtr future, int ec, IntPtr userData);

    [DllImport(_dllName, CharSet = CharSet.Ansi, ExactSpelling = true)]
    public static extern void nabto_client_future_set_callback(IntPtr future, [MarshalAs(UnmanagedType.FunctionPtr)] FutureCallbackFunc cb, IntPtr userData);


    /***********************/
    /****** COAP ***********/
    /***********************/
    [DllImport(_dllName, CharSet = CharSet.Ansi, ExactSpelling = true)]
    public static extern IntPtr nabto_client_coap_new(IntPtr connection, [MarshalAs(UnmanagedType.LPUTF8Str)] string method, [MarshalAs(UnmanagedType.LPUTF8Str)] string path);

    [DllImport(_dllName, CharSet = CharSet.Ansi, ExactSpelling = true)]
    public static extern void nabto_client_coap_free(IntPtr coap);

    [DllImport(_dllName, CharSet = CharSet.Ansi, ExactSpelling = true)]
    public static extern void nabto_client_coap_stop(IntPtr coap);

    // UIntPtr has the same number og bits as size_t
    [DllImport(_dllName, CharSet = CharSet.Ansi, ExactSpelling = true)]
    public static extern int nabto_client_coap_set_request_payload(IntPtr coap, ushort contentFormat, byte* payload, UIntPtr payloadLength);

    [DllImport(_dllName, CharSet = CharSet.Ansi, ExactSpelling = true)]
    public static extern void nabto_client_coap_execute(IntPtr coap, IntPtr future);

    [DllImport(_dllName, CharSet = CharSet.Ansi, ExactSpelling = true)]
    public static extern int nabto_client_coap_get_response_status_code(IntPtr coap, out ushort statusCode);

    [DllImport(_dllName, CharSet = CharSet.Ansi, ExactSpelling = true)]
    public static extern int nabto_client_coap_get_response_content_format(IntPtr coap, out ushort contentType);

    [DllImport(_dllName, CharSet = CharSet.Ansi, ExactSpelling = true)]
    public static extern int nabto_client_coap_get_response_payload(IntPtr coap, out byte* payload, out UIntPtr payloadLength);



    // util functions
    [DllImport(_dllName, CharSet = CharSet.Ansi, ExactSpelling = true)]
    public static extern void nabto_client_string_free(byte* s);


    // error handling
    [DllImport(_dllName, CharSet = CharSet.Ansi, ExactSpelling = true)]
    public static extern byte* nabto_client_error_get_message(int ec);


    [DllImport("libnabto_client.so", CharSet = CharSet.Ansi, SetLastError = true)]
    public static extern int NABTO_CLIENT_EC_OK_value();

    [DllImport("libnabto_client.so", CharSet = CharSet.Ansi, SetLastError = true)]
    public static extern int NABTO_CLIENT_EC_NOT_CONNECTED_value();

    [DllImport("libnabto_client.so", CharSet = CharSet.Ansi, SetLastError = true)]
    public static extern int NABTO_CLIENT_EC_INVALID_ARGUMENT_value();


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

    public static string GetVersion()
    {
        return constCharPointerToString(nabto_client_version());
    }

    public static string GetErrorMessage(int ec)
    {
        return constCharPointerToString(nabto_client_error_get_message(ec));
    }

    public static string LogMessageGetMessage(IntPtr logMessage)
    {
        return constCharPointerToString(nabto_client_log_message_get_message(logMessage));
    }


    public static int ConnectionGetClientFingerprint(IntPtr connection, out string fingerprint)
    {
        byte* data;
        int ec;
        fingerprint = "";
        ec = nabto_client_connection_get_client_fingerprint(connection, out data);
        if (ec == 0)
        {
            fingerprint = DecodeStringAndFree(data);
        }
        return ec;
    }

    public static int ConnectionGetDeviceFingerprint(IntPtr connection, out string fingerprint)
    {
        byte* data;
        int ec;
        fingerprint = "";
        ec = nabto_client_connection_get_device_fingerprint(connection, out data);
        if (ec == 0)
        {
            fingerprint = DecodeStringAndFree(data);
        }
        return ec;
    }

    public static int CreatePrivateKey(IntPtr client, out string privateKey)
    {
        byte* data;
        int ec;
        privateKey = "";
        ec = nabto_client_create_private_key(client, out data);
        if (ec == 0)
        {
            privateKey = DecodeStringAndFree(data);
        }
        return ec;
    }

    public static int CoapGetResponsePayload(IntPtr coap, out byte[] payload)
    {
        byte* p;
        UIntPtr pLength;
        int ec = nabto_client_coap_get_response_payload(coap, out p, out pLength);
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

    public static int CoapSetRequestPayload(IntPtr coap, ushort contentFormat, byte[] payload)
    {
        int ec;
        UIntPtr length = (UIntPtr)payload.Length;
        fixed(byte* start = &payload[0]) {
            ec = nabto_client_coap_set_request_payload(coap, contentFormat, start, length);
        }
        return ec;
    }
};
