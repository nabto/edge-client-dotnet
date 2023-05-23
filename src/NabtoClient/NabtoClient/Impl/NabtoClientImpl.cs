using Microsoft.Extensions.Logging;
using System.Runtime.InteropServices;

namespace Nabto.Edge.Client.Impl;

/// <inheritdoc/>
public class NabtoClientImpl : Nabto.Edge.Client.NabtoClient {
    private IntPtr _handle;
    private NabtoClientNative.LogCallbackFunc? _logCallback;

    internal bool _disposedUnmanaged;

    internal static NabtoClientImpl Create() {
        IntPtr ptr = Impl.NabtoClientNative.nabto_client_new();
        if (ptr == IntPtr.Zero) {
            throw new NullReferenceException();
        }
        return new NabtoClientImpl(ptr);
    }

    internal NabtoClientImpl(IntPtr h)
    {
        _handle = h;
    }

    /// <inheritdoc/>
    public void Dispose()
    {
        DisposeUnmanaged();
        GC.SuppressFinalize(this);
    }

    /// <inheritdoc/>
    public ValueTask DisposeAsync()
    {
        DisposeUnmanaged();
        GC.SuppressFinalize(this);
        return ValueTask.CompletedTask;
    }

    /// <inheritdoc/>
    ~NabtoClientImpl()
    {
        DisposeUnmanaged();
    }

    private void DisposeUnmanaged() {
        if (!_disposedUnmanaged) {
            NabtoClientNative.nabto_client_free(_handle);
        }
        _disposedUnmanaged = true;
    }


    /// <inheritdoc/>
    public IntPtr GetHandle() {
        if (_disposedUnmanaged) {
                throw new ObjectDisposedException("NabtoClient", "The NabtoClient instance has been disposed.");
        }
        return _handle;
    }

    /// <inheritdoc/>
    public string GetVersion() {
        return NabtoClientNative.nabto_client_version();
    }

    /// <inheritdoc/>
    public string CreatePrivateKey()
    {
        string privateKey;
        int ec = NabtoClientNative.nabto_client_create_private_key(GetHandle(), out privateKey);
        if (ec != 0) {
            throw NabtoExceptionFactory.Create(ec);
        }
        return privateKey;
    }

    /// <inheritdoc/>
    public Nabto.Edge.Client.Connection CreateConnection()
    {
        return Nabto.Edge.Client.Impl.ConnectionImpl.Create(this);
    }

    /// <inheritdoc/>
    public Nabto.Edge.Client.MdnsScanner CreateMdnsScanner(string subtype = "")
    {
        return Nabto.Edge.Client.Impl.MdnsScannerImpl.Create(this, subtype);
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

    /// <inheritdoc/>
    public void SetLogger(ILogger logger)
    {
        int ec = NabtoClientNative.nabto_client_set_log_level(GetHandle(), "trace");
        if (ec != 0) {
            throw NabtoExceptionFactory.Create(ec);
        }
        _logCallback =  (IntPtr logMessage, IntPtr ptr) => {
            LogLevel l = NabtoLogLevelToLogLevel(NabtoClientNative.nabto_client_log_message_get_severity(logMessage));
            string message = NabtoClientNative.nabto_client_log_message_get_message(logMessage);
            Microsoft.Extensions.Logging.LoggerExtensions.Log(logger, l, message);
        };
        ec = NabtoClientNative.nabto_client_set_log_callback(GetHandle(), _logCallback, IntPtr.Zero);
        //ec = NabtoClientNative.nabto_client_set_log_callback(GetHandle(), (ref NabtoClientNative.LogMessage logMessage, IntPtr ptr) =>  Microsoft.Extensions.Logging.LoggerExtensions.Log(logger, NabtoLogLevelToLogLevel(logMessage.severity), logMessage.message), IntPtr.Zero);
    }
}
