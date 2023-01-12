using Microsoft.Extensions.Logging;
using System.Runtime.InteropServices;

namespace Nabto.Edge.Client.Impl;


public class NabtoClient : Nabto.Edge.Client.NabtoClient {
    private IntPtr _handle;
    private NabtoClientNative.LogCallbackFunc? _logCallback;

    public static NabtoClient Create() {
        IntPtr ptr = Impl.NabtoClientNative.nabto_client_new();
        if (ptr == IntPtr.Zero) {
            throw new NullReferenceException();
        }
        return new NabtoClient(ptr);
    }

    public NabtoClient(IntPtr h)
    {
        _handle = h;
    }

    ~NabtoClient() {
        NabtoClientNative.nabto_client_free(_handle);
    }

    public IntPtr GetHandle() {
        return _handle;
    }

    public string GetVersion() {
        return NabtoClientNative.GetVersion();
    }

    public string CreatePrivateKey()
    {
        string privateKey;
        int ec = NabtoClientNative.CreatePrivateKey(GetHandle(), out privateKey);
        if (ec != 0) {
            throw NabtoException.Create(ec);
        }
        return privateKey;
    }

    public Nabto.Edge.Client.Connection CreateConnection()
    {
        return Nabto.Edge.Client.Impl.Connection.Create(this);
    }

    /*
    typedef enum NabtoClientLogSeverity_ {
    NABTO_CLIENT_LOG_SEVERITY_ERROR,
    NABTO_CLIENT_LOG_SEVERITY_WARN,
    NABTO_CLIENT_LOG_SEVERITY_INFO,
    NABTO_CLIENT_LOG_SEVERITY_DEBUG,
    NABTO_CLIENT_LOG_SEVERITY_TRACE,
} NabtoClientLogSeverity;
*/

    private LogLevel NabtoLogLevelToLogLevel(int level)
    {
        switch(level) {
            case 0: return LogLevel.Error;
            case 1: return LogLevel.Warning;
            case 2: return LogLevel.Information;
            case 3: return LogLevel.Debug;
            case 4: return LogLevel.Trace;
            default: return LogLevel.Error;
        }
    }

    public void SetLogger(ILogger logger)
    {
        int ec = NabtoClientNative.nabto_client_set_log_level(GetHandle(), "trace");
        if (ec != 0) {
            throw NabtoException.Create(ec);
        }
        _logCallback =  (IntPtr logMessage, IntPtr ptr) => {
            LogLevel l = NabtoLogLevelToLogLevel(NabtoClientNative.nabto_client_log_message_get_severity(logMessage));
            string message = NabtoClientNative.LogMessageGetMessage(logMessage);
            Microsoft.Extensions.Logging.LoggerExtensions.Log(logger, l, message);
        };
        ec = NabtoClientNative.nabto_client_set_log_callback(GetHandle(), _logCallback, IntPtr.Zero);
        //ec = NabtoClientNative.nabto_client_set_log_callback(GetHandle(), (ref NabtoClientNative.LogMessage logMessage, IntPtr ptr) =>  Microsoft.Extensions.Logging.LoggerExtensions.Log(logger, NabtoLogLevelToLogLevel(logMessage.severity), logMessage.message), IntPtr.Zero);
    }
}
