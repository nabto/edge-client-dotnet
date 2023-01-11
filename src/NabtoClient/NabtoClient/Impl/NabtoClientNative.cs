using System.Runtime.InteropServices;
using System.Text;

namespace Nabto.Edge.Client.Impl;

public unsafe class NabtoClientNative {
    [DllImport("libnabto_client.so", CharSet = CharSet.Ansi, SetLastError = true)]

    // not simply using a string as .net then deallocates the non heap allocated const char*.
    static extern byte* nabto_client_version();
    [DllImport("libnabto_client.so", CharSet = CharSet.Ansi, SetLastError = true)]


    // Nabto Client
    public static extern IntPtr nabto_client_new();
    [DllImport("libnabto_client.so", CharSet = CharSet.Ansi, SetLastError = true)]
    public static extern void nabto_client_free(IntPtr context);
    [DllImport("libnabto_client.so", CharSet = CharSet.Ansi, SetLastError = true)]
    public static extern void nabto_client_stop(IntPtr context);

    // Connection
    [DllImport("libnabto_client.so", CharSet = CharSet.Ansi, SetLastError = true)]
    public static extern IntPtr nabto_client_connection_new(IntPtr client);
    [DllImport("libnabto_client.so", CharSet = CharSet.Ansi, SetLastError = true)]
    public static extern void nabto_client_connection_free(IntPtr connection);

    [DllImport("libnabto_client.so", CharSet = CharSet.Ansi, SetLastError = true)]
    public static extern int nabto_client_connection_set_options(IntPtr connection, byte[] options);


    [DllImport("libnabto_client.so", CharSet = CharSet.Ansi, SetLastError = true)]
    public static extern int nabto_client_connection_get_client_fingerprint(IntPtr connection, out byte* fingerprint);

    [DllImport("libnabto_client.so", CharSet = CharSet.Ansi, SetLastError = true)]
    public static extern int nabto_client_connection_get_device_fingerprint(IntPtr connection, out byte* fingerprint);

    // util functions
    [DllImport("libnabto_client.so", CharSet = CharSet.Ansi, SetLastError = true)]
    public static extern void nabto_client_string_free(byte* s);


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

    public static string GetVersion() {
        return constCharPointerToString(nabto_client_version());
    }

    public static int ConnectionGetClientFingerprint(IntPtr connection, out string fingerprint)
    {
        byte* data;
        int ec;
        fingerprint = "";
        ec = nabto_client_connection_get_client_fingerprint(connection, out data);
        if (ec == 0) {
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
        if (ec == 0) {
            fingerprint = DecodeStringAndFree(data);
        }
        return ec;
    }

    public static int ConnectionSetOptions(IntPtr connection, string json)
    {
        byte[] bytes;
        UTF8Encoding utf8 = new UTF8Encoding();
        int byteCount = utf8.GetByteCount(json.ToCharArray(), 0, json.Length);
        bytes = new byte[byteCount+1];
        bytes[byteCount] = 0;
        int bytesEncodedCount = utf8.GetBytes(json, 0, json.Length, bytes, 0);
        return nabto_client_connection_set_options(connection, bytes);
    }
};
